﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zatwierdz_MM.Extensions
{
    public class ListViewExtensions
    {
        public static BindableProperty ScrollToProperty =
         BindableProperty.CreateAttached("ScrollTo", typeof(object),
                                     typeof(Nullable), null, propertyChanged: HandleChanged);

        public static object GetScrollTo(BindableObject view)
        {
            return view.GetValue(ScrollToProperty);
        }

        public static void SetScrollTo(BindableObject view, object cmd)
        {
            view.SetValue(ScrollToProperty, cmd);
        }

        static void HandleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var listView = bindable as ListView;

            if (listView == null)
                return;

            listView.ScrollTo(newValue, ScrollToPosition.Center, true);
        }
    }
}