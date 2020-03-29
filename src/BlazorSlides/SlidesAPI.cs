using System;

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

        public void MoveNext()
        {
            if (State.NextFragment() == false)
            {
                if (State.CurrentHorizontalIndex < State.HorizontalSlideCount - 1)
                {
                    State.CurrentHorizontalIndex++;
                    State.CurrentVerticalIndex = 0; //TODO: restore vertical index
                    State.CurrentFragmentIndex = -1;
                }
            }
            UpdateStatus();
        }

        public void MovePrevious()
        {
            if (State.PreviousFragment() == false)
            {
                if (State.CurrentHorizontalIndex != 0)
                {
                    State.CurrentHorizontalIndex--;
                    State.CurrentVerticalIndex = 0; //TODO: restore vertical index
                    State.CurrentFragmentIndex = State.CurrentFragmentIndex;
                }
            }
            UpdateStatus();
        }

        public void MoveUp()
        {
            if (State.PreviousFragment() == false)
            {
                if (State.CurrentVerticalIndex != 0)
                {
                    State.CurrentVerticalIndex--;
                    State.CurrentFragmentIndex = State.CurrentFragmentCount;
                }
            }
            UpdateStatus();
        }

        public void MoveDown()
        {
            if (State.NextFragment() == false)
            {
                if (State.CurrentVerticalIndex != State.VerticalSlideCount - 1)
                {
                    State.CurrentVerticalIndex++;
                    State.CurrentFragmentIndex = -1;
                }
            }
            UpdateStatus();
        }

        public void NavigateTo(int horizontal)
        {
            NavigateTo(horizontal, null);
        }

        public void NavigateTo(int horizontal, int? vertical)
        {
            bool changed = false;
            if(State.CurrentHorizontalIndex != horizontal)
            {
                State.CurrentHorizontalIndex = horizontal;
                changed = true;
            }
            if(vertical.HasValue)
            {
                if(State.CurrentVerticalIndex != vertical.Value)
                {
                    State.CurrentVerticalIndex = vertical.Value;
                    changed = true;
                }
            }
            else
            {
                if(State.CurrentVerticalIndex != 0)
                {
                    State.CurrentVerticalIndex = 0; //TODO: Restore index
                    changed = true;
                }
            }
            if(changed)
            {
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            StateUpdated?.Invoke(this, State);
        }
    }
}
