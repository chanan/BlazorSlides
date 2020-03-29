using BlazorSlides.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSlides
{
    public class State : EventArgs
    {
        //Private vars
        private List<ISlide> _slides = new List<ISlide>();

        //Public state
        public int HorizontalSlideCount => _slides.Count;
        public int VerticalSlideCount => _slides[CurrentHorizontalIndex] switch
        {
            InternalSlide _ => 0,
            InternalStack internalStack => internalStack.Slides.Count,
            _ => throw new NotImplementedException()
        };
        public int TotalSlideCount => _slides.Sum(slide => slide switch
        {
            InternalSlide _ => 1,
            InternalStack internalStack => internalStack.Slides.Count,
            _ => throw new NotImplementedException()
        });
        public int CurrentFragmentCount => _slides[CurrentHorizontalIndex] switch
        {
            InternalSlide slide => slide.Fragments.Count,
            InternalStack internalStack => internalStack.Slides[CurrentVerticalIndex].Fragments.Count,
            _ => throw new NotImplementedException()
        };
        public bool HasHorizontal => HorizontalSlideCount > 1;
        public bool HasVertical => _slides.Any(list => list is InternalStack);
        public int CurrentHorizontalIndex { get; internal set; } = 0;
        public int CurrentVerticalIndex { get; internal set; } = 0;
        public int CurrentFragmentIndex { get; internal set; } = -1;
        public int CurrentPastCount
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < HorizontalSlideCount; i++)
                {
                    if (i < CurrentHorizontalIndex)
                    {
                        ret += _slides[i] switch
                        {
                            InternalSlide _ => 1,
                            InternalStack internalStack => internalStack.Slides.Count,
                            _ => throw new NotImplementedException()
                        };
                    }
                    if (i == CurrentHorizontalIndex)
                    {
                        ret += CurrentVerticalIndex;
                    }
                }
                return ret;
            }
        }
        public int SlidesWidth { get; internal set; }
        public bool Ready { get; internal set; } = false;
        public bool HasNextSlide => CurrentHorizontalIndex < HorizontalSlideCount - 1;
        public bool HasPreviousSlide => CurrentHorizontalIndex > 0;
        public bool HasUpSlide => VerticalSlideCount != 0 && CurrentVerticalIndex > 0;
        public bool HasDownSlide => VerticalSlideCount != 0 && CurrentVerticalIndex < VerticalSlideCount - 1;
        public bool HasNextFragment => CurrentFragmentIndex < CurrentFragmentCount - 1;
        public bool HasPreviousFragment => CurrentFragmentIndex != -1;
        public bool IsVerticalSlide => VerticalSlideCount > 0;
        public ControlsBackArrows ControlsBackArrows { get; set; } = ControlsBackArrows.Faded;
        public ControlsLayout ControlsLayout { get; set; } = ControlsLayout.BottomRight;
        public bool ControlsTutorial { get; set; } = true;
        public bool Controls { get; set; } = true;
        public bool Progress { get; set; } = true;
        public bool SlideNumber { get; set; } = false;
        public SlideNumberFormat SlideNumberFormat { get; set; } = SlideNumberFormat.HorizontalPeriodVertical;
        public int Width { get; set; } = 960;
        public int Height { get; set; } = 700;
        public double Margin { get; set; } = 0.04d;
        public double MinScale { get; set; } = 0.2d;
        public double MaxScale { get; set; } = 2.00d;
        public Theme Theme { get; set; } = Theme.White;

        //Internal methods
        internal int AddHorizontalSlide()
        {
            _slides.Add(new InternalSlide { HorizontalIndex = _slides.Count });
            return HorizontalSlideCount - 1;
        }

        internal int AddStack()
        {
            _slides.Add(new InternalStack { HorizontalIndex = _slides.Count });
            return HorizontalSlideCount - 1;
        }

        internal int AddVerticalSlide(int horizontalIndex)
        {
            InternalStack stack = (InternalStack)_slides[horizontalIndex];
            int verticalIndex = stack.Slides.Count;
            stack.Slides.Add(new InternalSlide { HorizontalIndex = horizontalIndex, VerticalIndex = verticalIndex });
            return verticalIndex;
        }

        internal int AddFragment(int horizontalIndex, int? verticalIndex)
        {
            ISlide islide = _slides[horizontalIndex];
            InternalSlide internalSlide;
            if (verticalIndex.HasValue)
            {
                InternalStack internalStack = (InternalStack)islide;
                internalSlide = internalStack.Slides[verticalIndex.Value];
            }
            else
            {
                internalSlide = (InternalSlide)islide;
            }
            int fragmentIndex = internalSlide.Fragments.Count;
            internalSlide.Fragments.Add(new InternalFragment { FragmentIndex = fragmentIndex });
            return fragmentIndex;
        }

        internal bool NextFragment()
        {
            if (HasNextFragment)
            {
                CurrentFragmentIndex++;
                return true;
            }
            return false;
        }

        internal bool PreviousFragment()
        {
            if (HasPreviousFragment)
            {
                CurrentFragmentIndex--;
                return true;
            }
            return false;
        }
    }
}
