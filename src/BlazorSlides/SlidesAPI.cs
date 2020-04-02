using Microsoft.AspNetCore.Components;
using System;

namespace BlazorSlides
{
    public class SlidesAPI
    {
        public State State { get; } = new State();

        public event EventHandler<State> StateUpdated;

        internal int ResgisterHorizontalSlide(string id)
        {
            return State.AddHorizontalSlide(id);
        }

        internal int ResgisterStack()
        {
            return State.AddStack();
        }

        internal int ResgisterVerticalSlide(int horizontalIndex, string id)
        {
            return State.AddVerticalSlide(horizontalIndex, id);
        }

        internal int RegisterFragment(int horizontalIndex, int? verticalIndex)
        {
            return State.AddFragment(horizontalIndex, verticalIndex);
        }

        internal void UpdateSlideElementReference(int horizontalIndex, int? verticalIndex, ElementReference domSlide)
        {
            State.UpdateSlideElementReference(horizontalIndex, verticalIndex, domSlide);
        }

        public void MoveNext()
        {
            if (State.NextFragment() == false)
            {
                if (State.CurrentHorizontalIndex < State.HorizontalSlideCount - 1)
                {
                    State.CurrentHorizontalIndex++;
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
                    State.CurrentFragmentIndex = State.CurrentFragmentCount == 0 ? -1 : State.CurrentFragmentCount;
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
                    State.CurrentFragmentIndex = State.CurrentFragmentCount == 0 ? -1 : State.CurrentFragmentCount;
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
            if(changed)
            {
                UpdateStatus();
            }
        }

        internal void UpdateStatus()
        {
            StateUpdated?.Invoke(this, State);
        }
    }
}
