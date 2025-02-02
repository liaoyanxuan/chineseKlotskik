﻿using LeaderBoardSuperScrollView;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace LeaderBoardSuperScrollView
{
    public class LeaderBoardPullToLoadMoreDemoScript : MonoBehaviour
    {
        public LoopListView2 mLoopListView;
        LoadingTipStatus mLoadingTipStatus = LoadingTipStatus.None;
        float mLoadingTipItemHeight = 100;
        int mLoadMoreCount = 20;

      
        // Use this for initialization
        void Awake()
        {
            // totalItemCount +1 because the last "load more" banner is also a item.
            mLoopListView.InitListView(LeaderBoardDataSourceMgr.Get.TotalItemCount + 1, OnGetItemByIndex);
            mLoopListView.mOnBeginDragAction = OnBeginDrag;
            mLoopListView.mOnDragingAction = OnDraging;
            mLoopListView.mOnEndDragAction = OnEndDrag;

          
        }

        public void ShowBoardData()
        {
            mLoopListView.SetListItemCount(LeaderBoardDataSourceMgr.Get.TotalItemCount + 1, true);
            mLoopListView.RefreshAllShownItem();
            mLoopListView.ResetListView();


        }


        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0)
            {
                return null;
            }
            LoopListViewItem2 item = null;
            if (index == LeaderBoardDataSourceMgr.Get.TotalItemCount)
            {
                item = listView.NewListViewItem("ItemPrefab0");
                UpdateLoadingTip(item);
                return item;
            }
            ItemData itemData = LeaderBoardDataSourceMgr.Get.GetItemDataByIndex(index);
            if (itemData == null)
            {
                return null;
            }
            item = listView.NewListViewItem("ItemPrefab1");
            LeaderBoardListItem2 itemScript = item.GetComponent<LeaderBoardListItem2>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
                itemScript.Init();
            }
            if(index == LeaderBoardDataSourceMgr.Get.TotalItemCount -1)
            {
                item.Padding = 0;
            }
            itemScript.SetItemData(itemData, index);
            return item;
        }

        void UpdateLoadingTip(LoopListViewItem2 item)
        {
            if (item == null)
            {
                return;
            }
            ListItem0 itemScript0 = item.GetComponent<ListItem0>();
            if(itemScript0 == null)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.None)
            {
                itemScript0.mRoot.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitRelease)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mText.text = "加载更多";
                itemScript0.mArrow.SetActive(true);
                itemScript0.mWaitingIcon.SetActive(false);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
            }
            else if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                itemScript0.mRoot.SetActive(true);
                itemScript0.mArrow.SetActive(false);
                itemScript0.mWaitingIcon.SetActive(true);
                itemScript0.mText.text = "加载中 ...";
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
            }
        }

        void OnBeginDrag()
        {

        }
        void OnDraging()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus != LoadingTipStatus.None && mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(LeaderBoardDataSourceMgr.Get.TotalItemCount);
            if (item == null)
            {
                return;
            }
            LoopListViewItem2 item1 = mLoopListView.GetShownItemByItemIndex(LeaderBoardDataSourceMgr.Get.TotalItemCount-1);
            if (item1 == null)
            {
                return;
            }
            float y  = mLoopListView.GetItemCornerPosInViewPort(item1,ItemCornerEnum.LeftBottom).y;
            if(y + mLoopListView.ViewPortSize >= mLoadingTipItemHeight)
            {
                if (mLoadingTipStatus != LoadingTipStatus.None)
                {
                    return;
                }
                mLoadingTipStatus = LoadingTipStatus.WaitRelease;
                UpdateLoadingTip(item);
            }
            else
            {
                if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
                {
                    return;
                }
                mLoadingTipStatus = LoadingTipStatus.None;
                UpdateLoadingTip(item);
            }
        }
        void OnEndDrag()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus != LoadingTipStatus.None && mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            LoopListViewItem2 item = mLoopListView.GetShownItemByItemIndex(LeaderBoardDataSourceMgr.Get.TotalItemCount);
            if (item == null)
            {
                return;
            }
            mLoopListView.OnItemSizeChanged(item.ItemIndex);
            if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
            {
                return;
            }
            mLoadingTipStatus = LoadingTipStatus.WaitLoad;
            UpdateLoadingTip(item);
            LeaderBoardDataSourceMgr.Get.RequestLoadMoreDataList(mLoadMoreCount, OnDataSourceLoadMoreFinished);
        }

        void OnDataSourceLoadMoreFinished()
        {
            if (mLoopListView.ShownItemCount == 0)
            {
                return;
            }
            if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
            {
                mLoadingTipStatus = LoadingTipStatus.None;
                mLoopListView.SetListItemCount(LeaderBoardDataSourceMgr.Get.TotalItemCount + 1, false);
                mLoopListView.RefreshAllShownItem();
            }
        }

  

    }

}
