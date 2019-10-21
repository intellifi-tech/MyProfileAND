using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MyProfileAND.GenericClass
{
    class CenterZoomLayoutManager: LinearLayoutManager
    {
        private float mShrinkAmount = 0.15f;
        private float mShrinkDistance = 0.9f;

        public CenterZoomLayoutManager(Context context) : base(context)
        {

        }
        public CenterZoomLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
        {

        }
        public override int ScrollVerticallyBy(int dy, RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            int orientation = this.Orientation;
            if (orientation == Vertical)
            {
                int scrolled = base.ScrollVerticallyBy(dy, recycler, state);
                float midpoint = this.Height / 2f;
                float d0 = 0f;
                float d1 = mShrinkDistance * midpoint;
                float s0 = 1f;
                float s1 = 1f - mShrinkAmount;
                for (int i = 0; i < this.ChildCount; i++)
                {
                    View child = this.GetChildAt(i);// getChildAt(i);
                    float childMidpoint =
                            (GetDecoratedBottom(child) + GetDecoratedTop(child)) / 2f;
                    float d = Math.Min(d1, Math.Abs(midpoint - childMidpoint));
                    float scale = s0 + (s1 - s0) * (d - d0) / (d1 - d0);
                    child.ScaleX=scale;
                    child.ScaleY = scale;
                }
                return scrolled;
            }
            else
            {
                return 0;
            }
        }
        public override int ScrollHorizontallyBy(int dx, RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            int orientation = this.Orientation;
            if (orientation == Horizontal)
            {
                int scrolled = base.ScrollHorizontallyBy(dx, recycler, state);

                float midpoint = Width/ 2f;
                float d0 = 0f;
                float d1 = mShrinkDistance * midpoint;
                float s0 = 1f;
                float s1 = 1f - mShrinkAmount;
                for (int i = 0; i <this.ChildCount; i++)
                {
                    View child = GetChildAt(i);
                    float childMidpoint =
                            (GetDecoratedRight(child) + GetDecoratedLeft(child)) / 2f;
                    float d = Math.Min(d1, Math.Abs(midpoint - childMidpoint));
                    float scale = s0 + (s1 - s0) * (d - d0) / (d1 - d0);
                    child.ScaleX = scale+(scale *0.1f);
                    child.ScaleY = scale+(scale * 0.1f); 
                }
                return scrolled;
            }
            else
            {
                return 0;
            }
        }
    }
}