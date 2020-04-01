using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace BlazorSlides.Internal.Components
{
    public partial class Progress : ComponentBase
    {
        //Styles
        private string _progress;
        private string _span;

        //Injections
        [CascadingParameter(Name = "SlidesAPI")] public SlidesAPI SlidesAPI { get; set; }

        public string ProgressWidth
        {
            get
            {
                double progress = GetProgress();
                progress *= SlidesAPI.State.ComputedSize.Width;
                return progress.ToString("F2") + "px;";
            }
        }

        private double GetProgress()
        {
            double totalCount = (double)SlidesAPI.State.TotalSlideCount;
            double pastCount = (double)SlidesAPI.State.CurrentPastCount;
            double allFragments = (double)SlidesAPI.State.CurrentFragmentCount;
            if(allFragments > 0)
            {
                double visibleFragments = (double)SlidesAPI.State.CurrentFragmentIndex + 1;
                double fragmentWeight = 0.9d;
                pastCount += (visibleFragments / allFragments) * fragmentWeight;
            }
            return Math.Min(pastCount / (totalCount - 1), 1d);
        }

        //Events
        private void _onClick(MouseEventArgs e)
        {
            int slidesTotal = SlidesAPI.State.HorizontalSlideCount;
            int slideIndex = (int)Math.Floor((double)( e.ClientX / SlidesAPI.State.ComputedSize.Width) * slidesTotal);
            SlidesAPI.NavigateTo(slideIndex);
        }
    }
}
