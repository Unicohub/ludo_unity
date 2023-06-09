/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

namespace Facebook.Unity.Mobile.IOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class IOSFacebook : MobileFacebook
    {
        private const string CancelledResponse = "{\"cancelled\":true}";
        private bool limitEventUsage;
        private IIOSWrapper iosWrapper;

        public IOSFacebook()
            : this(new IOSWrapper(), new CallbackManager())
        {
        }

        public IOSFacebook(IIOSWrapper iosWrapper, CallbackManager callbackManager)
            : base(callbackManager)
        {
            this.iosWrapper = iosWrapper;
        }

        public enum FBInsightsFlushBehavior
        {
            /// <summary>
            /// The FB insights flush behavior auto.
            /// </summary>
            FBInsightsFlushBehaviorAuto,

            /// <summary>
            /// The FB insights flush behavior explicit only.
            /// </summary>
            FBInsightsFlushBehaviorExplicitOnly,
        }

        public override bool LimitEventUsage
        {
            get
            {
                return this.limitEventUsage;
            }

            set
            {
                this.limitEventUsage = value;
                this.iosWrapper.FBAppEventsSetLimitEventUsage(value);
            }
        }

        public override string SDKName
        {
            get
            {
                return "FBiOSSDK";
            }
        }

        public override string SDKVersion
        {
            get
            {
                return this.iosWrapper.FBSdkVersion();
            }
        }

        public void Init(
            string appId,
            bool frictionlessRequests,
            HideUnityDelegate hideUnityDelegate,
            InitDelegate onInitComplete)
        {
            base.Init(
                hideUnityDelegate,
                onInitComplete);

            this.iosWrapper.Init(
                appId,
                frictionlessRequests,
                FacebookSettings.IosURLSuffix,
                Constants.UnitySDKUserAgentSuffixLegacy);
        }

        public override void LogInWithReadPermissions(
            IEnumerable<string> permissions,
            FacebookDelegate<ILoginResult> callback)
        {
            this.iosWrapper.LogInWithReadPermissions(this.AddCallback(callback), permissions.ToCommaSeparateList());
        }

        public override void LogInWithPublishPermissions(
            IEnumerable<string> permissions,
            FacebookDelegate<ILoginResult> callback)
        {
            this.iosWrapper.LogInWithPublishPermissions(this.AddCallback(callback), permissions.ToCommaSeparateList());
        }

        public override void LogOut()
        {
            base.LogOut();
            this.iosWrapper.LogOut();
        }

        public override void AppRequest(
            string message,
            OGActionType? actionType,
            string objectId,
            IEnumerable<string> to,
            IEnumerable<object> filters,
            IEnumerable<string> excludeIds,
            int? maxRecipients,
            string data,
            string title,
            FacebookDelegate<IAppRequestResult> callback)
        {
            this.ValidateAppRequestArgs(
                message,
                actionType,
                objectId,
                to,
                filters,
                excludeIds,
                maxRecipients,
                data,
                title,
                callback);

            string mobileFilter = null;
            if (filters != null && filters.Any())
            {
                mobileFilter = filters.First() as string;
            }

            this.iosWrapper.AppRequest(
                this.AddCallback(callback),
                message,
                (actionType != null) ? actionType.ToString() : string.Empty,
                objectId != null ? objectId : string.Empty,
                to != null ? to.ToArray() : null,
                to != null ? to.Count() : 0,
                mobileFilter != null ? mobileFilter : string.Empty,
                excludeIds != null ? excludeIds.ToArray() : null,
                excludeIds != null ? excludeIds.Count() : 0,
                maxRecipients.HasValue,
                maxRecipients.HasValue ? maxRecipients.Value : 0,
                data,
                title);
        }

        public override void AppInvite(
            Uri appLinkUrl,
            Uri previewImageUrl,
            FacebookDelegate<IAppInviteResult> callback)
        {
            string appLinkUrlStr = string.Empty;
            string previewImageUrlStr = string.Empty;
            if (appLinkUrl != null && !string.IsNullOrEmpty(appLinkUrl.AbsoluteUri))
            {
                appLinkUrlStr = appLinkUrl.AbsoluteUri;
            }

            if (previewImageUrl != null && !string.IsNullOrEmpty(previewImageUrl.AbsoluteUri))
            {
                previewImageUrlStr = previewImageUrl.AbsoluteUri;
            }

            this.iosWrapper.AppInvite(
                this.AddCallback(callback),
                appLinkUrlStr,
                previewImageUrlStr);
        }

        public override void ShareLink(
            Uri contentURL,
            string contentTitle,
            string contentDescription,
            Uri photoURL,
            FacebookDelegate<IShareResult> callback)
        {
            this.iosWrapper.ShareLink(
                this.AddCallback(callback),
                contentURL.AbsoluteUrlOrEmptyString(),
                contentTitle,
                contentDescription,
                photoURL.AbsoluteUrlOrEmptyString());
        }

        public override void FeedShare(
            string toId,
            Uri link,
            string linkName,
            string linkCaption,
            string linkDescription,
            Uri picture,
            string mediaSource,
            FacebookDelegate<IShareResult> callback)
        {
            string linkStr = link != null ? link.ToString() : string.Empty;
            string pictureStr = picture != null ? picture.ToString() : string.Empty;
            this.iosWrapper.FeedShare(
                this.AddCallback(callback),
                toId,
                linkStr,
                linkName,
                linkCaption,
                linkDescription,
                pictureStr,
                mediaSource);
        }

        public override void GameGroupCreate(
            string name,
            string description,
            string privacy,
            FacebookDelegate<IGroupCreateResult> callback)
        {
            this.iosWrapper.CreateGameGroup(this.AddCallback(callback), name, description, privacy);
        }

        public override void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback)
        {
            this.iosWrapper.JoinGameGroup(System.Convert.ToInt32(CallbackManager.AddFacebookDelegate(callback)), id);
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters)
        {
            NativeDict dict = MarshallDict(parameters);
            if (valueToSum.HasValue)
            {
                this.iosWrapper.LogAppEvent(logEvent, valueToSum.Value, dict.NumEntries, dict.Keys, dict.Values);
            }
            else
            {
                this.iosWrapper.LogAppEvent(logEvent, 0.0, dict.NumEntries, dict.Keys, dict.Values);
            }
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters)
        {
            NativeDict dict = MarshallDict(parameters);
            this.iosWrapper.LogPurchaseAppEvent(logPurchase, currency, dict.NumEntries, dict.Keys, dict.Values);
        }

        public override void ActivateApp(string appId)
        {
            // Activate app is logged automatically on ios.
        }

        public override void FetchDeferredAppLink(FacebookDelegate<IAppLinkResult> callback)
        {
            this.iosWrapper.FetchDeferredAppLink(this.AddCallback(callback));
        }

        public override void GetAppLink(
            FacebookDelegate<IAppLinkResult> callback)
        {
            this.iosWrapper.GetAppLink(System.Convert.ToInt32(CallbackManager.AddFacebookDelegate(callback)));
        }

        public override void RefreshCurrentAccessToken(
            FacebookDelegate<IAccessTokenRefreshResult> callback)
        {
            this.iosWrapper.RefreshCurrentAccessToken(
                System.Convert.ToInt32(CallbackManager.AddFacebookDelegate(callback)));
        }

        protected override void SetShareDialogMode(ShareDialogMode mode)
        {
            this.iosWrapper.SetShareDialogMode((int)mode);
        }

        private static NativeDict MarshallDict(Dictionary<string, object> dict)
        {
            NativeDict res = new NativeDict();
            
            if (dict != null && dict.Count > 0)
            {
                res.Keys = new string[dict.Count];
                res.Values = new string[dict.Count];
                res.NumEntries = 0;
                foreach (KeyValuePair<string, object> kvp in dict)
                {
                    res.Keys[res.NumEntries] = kvp.Key;
                    res.Values[res.NumEntries] = kvp.Value.ToString();
                    res.NumEntries++;
                }
            }
            
            return res;
        }
        
        private static NativeDict MarshallDict(Dictionary<string, string> dict)
        {
            NativeDict res = new NativeDict();
            
            if (dict != null && dict.Count > 0)
            {
                res.Keys = new string[dict.Count];
                res.Values = new string[dict.Count];
                res.NumEntries = 0;
                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    res.Keys[res.NumEntries] = kvp.Key;
                    res.Values[res.NumEntries] = kvp.Value;
                    res.NumEntries++;
                }
            }
            
            return res;
        }
        
        private int AddCallback<T>(FacebookDelegate<T> callback) where T : IResult
        {
            string asyncId = this.CallbackManager.AddFacebookDelegate(callback);
            return Convert.ToInt32(asyncId);
        }

        private class NativeDict
        {
            public NativeDict()
            {
                this.NumEntries = 0;
                this.Keys = null;
                this.Values = null;
            }

            public int NumEntries { get; set; }

            public string[] Keys { get; set; }

            public string[] Values { get; set; }
        }
    }
}
