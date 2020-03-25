using System;
using System.Threading.Tasks;

namespace BlazorSlides
{
    public class SlidesAPI
    {
        public State State { get; } = new State();

        public event EventHandler<State> StateUpdated;

        internal int ResgisterHorizontalSlide()
        {
            return State.AddHorizontalSlide();
        }

        internal int ResgisterStack()
        {
            return State.AddStack();
        }

        internal int ResgisterVerticalSlide(int horizontalIndex)
        {
            return State.AddVerticalSlide(horizontalIndex);
        }

        internal int RegisterFragment(int horizontalIndex, int? verticalIndex)
        {
            return State.AddFragment(horizontalIndex, verticalIndex);
        }

        public async Task MoveNext()
        {
            if (NextFragment() == false)
            {
                if (State.CurrentHorizontalIndex < State.HorizontalSlideCount - 1)
                {
                    State.CurrentHorizontalIndex++;
                    State.CurrentVerticalIndex = 0; //TODO: restore vertical index
                    State.CurrentFragmentIndex = -1;
                }
                //UpdateVerticalState();
            }
            //await UpdateJsInteropVars();
            StateUpdated?.Invoke(this, State); //TODO: BeginInvoke...
        }

        private bool NextFragment()
        {
            //SlideWithContent slide = (SlideWithContent)GetCurrentSlide();
            //return slide.NextFragment();
            return false;
        }

        public async Task MovePrevious()
        {
            if (PreviousFragment() == false)
            {
                if (State.CurrentHorizontalIndex != 0)
                {
                    State.CurrentHorizontalIndex--;
                    State.CurrentVerticalIndex = 0; //TODO: restore vertical index
                }
                //UpdateVerticalState();
            }
            //await UpdateJsInteropVars();
            StateUpdated?.Invoke(this, State);
        }

        private bool PreviousFragment()
        {
            //SlideWithContent slide = (SlideWithContent)GetCurrentSlide();
            //return slide.PreviousFragment();
            return false;
        }

        public async Task MoveUp()
        {
            if (State.CurrentVerticalIndex != 0)
            {
                State.CurrentVerticalIndex--;
            }
            //await UpdateJsInteropVars();
            StateUpdated?.Invoke(this, State);
        }

        public async Task MoveDown()
        {
            if (State.CurrentVerticalIndex != State.VerticalSlideCount - 1)
            {
                State.CurrentVerticalIndex++;
            }
            //await UpdateJsInteropVars();
            StateUpdated?.Invoke(this, State);
        }
    }
}
