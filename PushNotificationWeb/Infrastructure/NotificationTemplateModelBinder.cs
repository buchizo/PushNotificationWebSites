namespace PushNotification.WebSites.Web.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using NotificationsExtensions.BadgeContent;
    using NotificationsExtensions.RawContent;
    using NotificationsExtensions.TileContent;
    using NotificationsExtensions.ToastContent;

    public class NotificationTemplateModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var notificationType = bindingContext.ValueProvider.GetValue("NotificationType").AttemptedValue;

            Type factoryType = null;
            switch (notificationType)
            {
                case "Badge":
                    factoryType = typeof(BadgeContentFactory);
                    break;
                case "Tile":
                    factoryType = typeof(TileContentFactory);
                    break;
                case "Toast":
                    factoryType = typeof(ToastContentFactory);
                    break;
                case "Raw":
                    factoryType = typeof(RawContentFactory);
                    break;
            }

            if (factoryType == null)
            {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }

            var notificationTemplateType = bindingContext.ValueProvider.GetValue("NotificationTemplateType").AttemptedValue;
            var factory = factoryType.GetMethod("Create" + notificationTemplateType);
            var model = factory.Invoke(null, null);

            SetDefaultValues(bindingContext, model);

            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, model.GetType());
            return model;
        }

        private static void SetDefaultValues(ModelBindingContext bindingContext, object model)
        {
            // When sending a wide tile in a notification, the payload also includes a square tile 
            // because it is not guaranteed which will be displayed; the user has the option to choose 
            // either representation in their Start screen.
            var wideTile = model as IWideTileNotificationContent;
            if (wideTile != null)
            {

                var tileImageUrl = bindingContext.ValueProvider.GetValue("AlternativeTileImage");
                var tileText = bindingContext.ValueProvider.GetValue("AlternativeTileText");

                if (tileImageUrl != null && tileText != null)
                {
                    var squareTile = TileContentFactory.CreateTileSquarePeekImageAndText04();
                    squareTile.Image.Src = tileImageUrl.AttemptedValue;
                    squareTile.TextBodyWrap.Text = tileText.AttemptedValue;
                    wideTile.SquareContent = squareTile;
                    return;
                }

                if (tileImageUrl != null)
                {
                    var squareTile = TileContentFactory.CreateTileSquareImage();
                    squareTile.Image.Src = tileImageUrl.AttemptedValue;
                    wideTile.SquareContent = squareTile;
                    return;
                }

                if (tileText != null)
                {
                    var squareTile = TileContentFactory.CreateTileSquareText04();
                    squareTile.TextBodyWrap.Text = tileText.AttemptedValue;
                    wideTile.SquareContent = squareTile;
                }
            }

            // When sending a toast notification that has looping audio src, the Audio.Loop property should be set to true
            // and the Duration property should be set to Long, as specified by the WNS model.
            var toast = model as IToastNotificationContent;
            if (toast != null)
            {
                var result = bindingContext.ValueProvider.GetValue("Audio.Content");
                if (result != null)
                {
                    ToastAudioContent audioContent;
                    if (Enum.TryParse<ToastAudioContent>(result.AttemptedValue, out audioContent))
                    {
                        toast.Audio.Loop = audioContent == ToastAudioContent.LoopingAlarm || audioContent == ToastAudioContent.LoopingAlarm2 ||
                                            audioContent == ToastAudioContent.LoopingCall || audioContent == ToastAudioContent.LoopingCall2;
                        if (toast.Audio.Loop)
                        {
                            toast.Duration = ToastDuration.Long;
                        }
                    }
                }
            }
        }
    }
}