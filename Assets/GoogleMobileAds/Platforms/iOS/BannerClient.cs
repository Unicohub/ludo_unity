/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.iOS
{
    internal class BannerClient : IBannerClient, IDisposable
    {
        #region Banner callback types

        internal delegate void GADUAdViewDidReceiveAdCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, string error);
        internal delegate void GADUAdViewWillPresentScreenCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewDidDismissScreenCallback(IntPtr bannerClient);
        internal delegate void GADUAdViewWillLeaveApplicationCallback(IntPtr bannerClient);

        #endregion

        public event EventHandler<EventArgs> OnAdLoaded = delegate {};
        public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate {};
        public event EventHandler<EventArgs> OnAdOpening = delegate {};
        public event EventHandler<EventArgs> OnAdClosed = delegate {};
        public event EventHandler<EventArgs> OnAdLeavingApplication = delegate {};

        private IntPtr bannerViewPtr;

        // This property should be used when setting the bannerViewPtr.
        private IntPtr BannerViewPtr
        {
            get
            {
                return bannerViewPtr;
            }
            set
            {
                Externs.GADURelease(bannerViewPtr);
                bannerViewPtr = value;
            }
        }

        #region IBannerClient implementation

        // Creates a banner view.
        public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position) {
            IntPtr bannerClientPtr = (IntPtr) GCHandle.Alloc(this);

            if (adSize.IsSmartBanner) {
                BannerViewPtr = Externs.GADUCreateSmartBannerView(
                        bannerClientPtr, adUnitId, (int)position);
            }
            else
            {
                BannerViewPtr = Externs.GADUCreateBannerView(
                        bannerClientPtr, adUnitId, adSize.Width, adSize.Height, (int)position);
            }
            Externs.GADUSetBannerCallbacks(
                    BannerViewPtr,
                    AdViewDidReceiveAdCallback,
                    AdViewDidFailToReceiveAdWithErrorCallback,
                    AdViewWillPresentScreenCallback,
                    AdViewDidDismissScreenCallback,
                    AdViewWillLeaveApplicationCallback);
        }

        // Loads an ad.
        public void LoadAd(AdRequest request)
        {
            IntPtr requestPtr = Utils.BuildAdRequest(request);
            Externs.GADURequestBannerAd(BannerViewPtr, requestPtr);
            Externs.GADURelease(requestPtr);
        }

        // Displays the banner view on the screen.
        public void ShowBannerView() {
            Externs.GADUShowBannerView(BannerViewPtr);
        }

        // Hides the banner view from the screen.
        public void HideBannerView()
        {
            Externs.GADUHideBannerView(BannerViewPtr);
        }

        // Destroys the banner view.
        public void DestroyBannerView()
        {
            Externs.GADURemoveBannerView(BannerViewPtr);
            BannerViewPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            DestroyBannerView();
            ((GCHandle)bannerViewPtr).Free();
        }

        ~BannerClient()
        {
            Dispose();
        }

        #endregion

        #region Banner callback methods

        [MonoPInvokeCallback(typeof(GADUAdViewDidReceiveAdCallback))]
        private static void AdViewDidReceiveAdCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            client.OnAdLoaded(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidFailToReceiveAdWithErrorCallback))]
        private static void AdViewDidFailToReceiveAdWithErrorCallback(
                IntPtr bannerClient, string error)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            AdFailedToLoadEventArgs args = new AdFailedToLoadEventArgs() {
                Message = error
            };
            client.OnAdFailedToLoad(client, args);
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillPresentScreenCallback))]
        private static void AdViewWillPresentScreenCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            client.OnAdOpening(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADUAdViewDidDismissScreenCallback))]
        private static void AdViewDidDismissScreenCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            client.OnAdClosed(client, EventArgs.Empty);
        }

        [MonoPInvokeCallback(typeof(GADUAdViewWillLeaveApplicationCallback))]
        private static void AdViewWillLeaveApplicationCallback(IntPtr bannerClient)
        {
            BannerClient client = IntPtrToBannerClient(bannerClient);
            client.OnAdLeavingApplication(client, EventArgs.Empty);
        }

        private static BannerClient IntPtrToBannerClient(IntPtr bannerClient)
        {
            GCHandle handle = (GCHandle) bannerClient;
            return handle.Target as BannerClient;
        }

        #endregion
    }
}
