using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSlides
{
    public class State : EventArgs
    {
        //Private vars
        private List<List<bool>> _map = new List<List<bool>>();

        //Public state
        public int HorizontalSlideCount { get => _map.Count; }
        public int VerticalSlideCount { get => _map[CurrentHorizontalIndex].Count == 1 ? 0 : _map[CurrentHorizontalIndex].Count; }
        public int TotalSlideCount { get => _map.Sum(list => list.Count); }
        public bool HasHorizontal { get => HorizontalSlideCount > 1; }
        public bool HasVertical { get => _map.Any(list => list.Count > 1); }
        public int CurrentHorizontalIndex { get; internal set; } = 0;
        public int CurrentVerticalIndex { get; internal set; } = 0;
        public int CurrentPastCount {
            get {
                int ret = 0;
                for (int i = 0; i < HorizontalSlideCount; i++)
                {
                    if(i < CurrentHorizontalIndex)
                    {
                        ret += _map[i].Count;
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

        //Internal methods
        internal int AddHorizontalSlide()
        {
            _map.Add(new List<bool> { true });
            return HorizontalSlideCount - 1;
        }

        internal int AddStack()
        {
            _map.Add(new List<bool>());
            return HorizontalSlideCount - 1;
        }

        internal int AddVerticalSlide(int horizontalIndex)
        {
            List<bool> list = _map[horizontalIndex];
            list.Add(true);
            return list.Count - 1;
        }
    }
}
