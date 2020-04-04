using BlazorSlides.Internal;
using Microsoft.AspNetCore.Components;
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
        public int CurrentVerticalIndex { 
            get 
            {
                return IsVerticalSlide ? ((InternalStack)_slides[CurrentHorizontalIndex]).VerticalIndex.Value : 0;
            }

            internal set 
            { 
                if(IsVerticalSlide)
                {
                    ((InternalStack)_slides[CurrentHorizontalIndex]).VerticalIndex = value;
                }
            }
        }
        internal InternalSlide CurrentSlide => GetSlide(CurrentHorizontalIndex, CurrentVerticalIndex);
        public string CurrentSlideId => CurrentSlide.Id;
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
        public bool Ready { get; internal set; } = false;
        public bool HasNextSlide => CurrentHorizontalIndex < HorizontalSlideCount - 1;
        public bool HasPreviousSlide => CurrentHorizontalIndex > 0;
        public bool HasUpSlide => VerticalSlideCount != 0 && CurrentVerticalIndex > 0;
        public bool HasDownSlide => VerticalSlideCount != 0 && CurrentVerticalIndex < VerticalSlideCount - 1;
        public bool HasNextFragment => CurrentFragmentIndex < CurrentFragmentCount - 1;
        public bool HasPreviousFragment => CurrentFragmentIndex != -1;
        public bool IsVerticalSlide => VerticalSlideCount > 0;
        public ControlsBackArrows ControlsBackArrows { get; internal set; } = ControlsBackArrows.Faded;
        public ControlsLayout ControlsLayout { get; internal set; } = ControlsLayout.BottomRight;
        public bool ControlsTutorial { get; internal set; } = true;
        public bool Controls { get; internal set; } = true;
        public bool Progress { get; internal set; } = true;
        public bool SlideNumber { get; internal set; } = false;
        public SlideNumberFormat SlideNumberFormat { get; internal set; } = SlideNumberFormat.HorizontalPeriodVertical;
        public int Width { get; internal set; } = 960;
        public int Height { get; internal set; } = 700;
        public double Margin { get; internal set; } = 0.04d;
        public double MinScale { get; internal set; } = 0.2d;
        public double MaxScale { get; internal set; } = 2.00d;
        public Theme Theme { get; internal set; } = Theme.Black;
        public Transition Transition { get; internal set; } = Transition.Slide;
        public Size ComputedSize { get; internal set; } = new Size();
        internal IEnumerable<InternalSlide> Slides
        {
            get
            {
                foreach (ISlide islide in _slides)
                {
                    switch (islide)
                    {
                        case InternalSlide slide:
                            yield return slide;
                            break;
                        case InternalStack stack:
                            foreach (InternalSlide slide in stack.Slides)
                            {
                                yield return slide;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        //Internal methods
        internal int AddHorizontalSlide(string id)
        {
            _slides.Add(new InternalSlide { HorizontalIndex = _slides.Count, Id = id });
            return HorizontalSlideCount - 1;
        }

        internal int AddStack()
        {
            _slides.Add(new InternalStack { HorizontalIndex = _slides.Count });
            return HorizontalSlideCount - 1;
        }

        internal int AddVerticalSlide(int horizontalIndex, string id)
        {
            InternalStack stack = (InternalStack)_slides[horizontalIndex];
            int verticalIndex = stack.Slides.Count;
            stack.Slides.Add(new InternalSlide { HorizontalIndex = horizontalIndex, VerticalIndex = verticalIndex, Id = id });
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

        internal bool TryGetHorizontalById(string id, out int res)
        {
            bool found = false;
            int index = 0;
            foreach(ISlide slide in _slides)
            {
                switch(slide)
                {
                    case InternalSlide internalSlide:
                        if(internalSlide.Id == id)
                        {
                            found = true;
                            index = internalSlide.HorizontalIndex;
                            break;
                        }
                        break;
                    case InternalStack stack:
                        foreach(InternalSlide internalSlide in stack.Slides)
                        {
                            if (internalSlide.Id == id)
                            {
                                found = true;
                                index = internalSlide.HorizontalIndex;
                                break;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            res = index;
            return found;
        }

        internal void UpdateSlideElementReference(int horizontalIndex, int? verticalIndex, ElementReference domSlide)
        {
            ISlide islide = _slides[horizontalIndex];
            if (verticalIndex.HasValue)
            {
                InternalStack internalStack = (InternalStack)islide;
                InternalSlide internalSlide = internalStack.Slides[verticalIndex.Value];
                internalSlide.ElementReference = domSlide;
            }
            else
            {
                InternalSlide internalSlide = (InternalSlide)islide;
                internalSlide.ElementReference = domSlide;
            }
        }

        internal InternalSlide GetSlide(int horizontalIndex, int? verticalIndex)
        {
            return _slides[horizontalIndex] switch
            {
                InternalSlide slide => slide,
                InternalStack stack => stack.Slides[verticalIndex.Value],
                _ => throw new NotImplementedException()
            };
        }
    }
}
