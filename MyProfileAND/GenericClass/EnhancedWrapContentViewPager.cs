using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MyProfileAND.GenericClass
{
    class EnhancedWrapContentViewPager : ViewPager
    {
        public EnhancedWrapContentViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public EnhancedWrapContentViewPager(Context context) : base(context)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            MeasureSpecMode mode = MeasureSpec.GetMode(heightMeasureSpec);
            if (mode == MeasureSpecMode.Unspecified || mode == MeasureSpecMode.AtMost)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

                int height = 0;
                for (int i = 0; i < this.ChildCount; i++)
                {
                    View child = GetChildAt(i);
                    child.Measure(widthMeasureSpec, MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                    int childMeasuredHeight = child.MeasuredHeight;
                    if (childMeasuredHeight > height)
                    {
                        height = childMeasuredHeight;
                    }
                }
                heightMeasureSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
            }
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }
    }
}