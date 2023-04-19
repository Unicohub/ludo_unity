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

#if UNITY_ANDROID

using System;
using UnityEngine;

using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Android
{
    internal class DefaultInAppPurchaseListener : AndroidJavaProxy
    {
        public IDefaultInAppPurchaseProcessor inAppPurchaseProcessor;

        internal DefaultInAppPurchaseListener(IDefaultInAppPurchaseProcessor inAppPurchaseProcessor)
            : base(Utils.PlayStorePurchaseListenerClassName)
        {
            this.inAppPurchaseProcessor = inAppPurchaseProcessor;
        }

        bool isValidPurchase(string sku) {
            return inAppPurchaseProcessor.IsValidPurchase(sku);
        }

        void onInAppPurchaseFinished(AndroidJavaObject result) {
            InAppPurchaseResult wrappedResult = new InAppPurchaseResult(result);
            if (wrappedResult.IsSuccessful && wrappedResult.IsVerified) {
                inAppPurchaseProcessor.ProcessCompletedInAppPurchase(wrappedResult);
            }
        }
    }
}

#endif
