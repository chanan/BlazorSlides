using Microsoft.AspNetCore.Components;
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
                double progress = Math.Min(SlidesAPI.State.CurrentPastCount / (double)(SlidesAPI.State.TotalSlideCount - 1), 1);
                progress *= SlidesAPI.State.SlidesWidth;
                return progress.ToString("F2") + "px;";
            }
        }
        /*
            TODO:
            * Progress click
            * Current slide fragments
            * offsetWidth

        function getProgress()
        {

            // The number of past and total slides
            var totalCount = getTotalSlides();
            var pastCount = getSlidePastCount();

            if (currentSlide)
            {

                var allFragments = currentSlide.querySelectorAll('.fragment');

                // If there are fragments in the current slide those should be
                // accounted for in the progress.
                if (allFragments.length > 0)
                {
                    var visibleFragments = currentSlide.querySelectorAll('.fragment.visible');

                    // This value represents how big a portion of the slide progress
                    // that is made up by its fragments (0-1)
                    var fragmentWeight = 0.9;

                    // Add fragment progress to the past slide count
                    pastCount += (visibleFragments.length / allFragments.length) * fragmentWeight;
                }

            }

            return Math.min(pastCount / (totalCount - 1), 1);

        }*/
    }
}
