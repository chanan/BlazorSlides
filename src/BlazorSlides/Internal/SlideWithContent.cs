using System.Collections.Generic;
using System.Linq;

namespace BlazorSlides.Internal
{
    internal abstract class SlideWithContent
    {
        private IEnumerable<IContent> _contents;
        public IEnumerable<IContent> Contents
        {
            get
            {
                int i = 0;
                foreach (IContent item in _contents)
                {
                    if (item is FragmentContent)
                    {
                        FragmentContent fc = (FragmentContent)item;
                        fc.IsCurrent = CurrentFragment == i + 1;
                        fc.IsPast = CurrentFragment > i + 1;
                        i++;
                    }
                    yield return item;
                }
            }

            set
            {
                _contents = value;
            }
        }

        public bool HasFragment() {
            return Contents.Any(content => content is FragmentContent);
        }

        public int FragmentCount() {
            return Contents.ToList().FindAll(content => content is FragmentContent).Count();
        }

        public int CurrentFragment { get; private set; } = 0;

        public bool NextFragment() {
            if (HasFragment() && CurrentFragment < FragmentCount()) {
                CurrentFragment++;
                return true;
            }
            return false;
        }

        public bool PreviousFragment() {
            if(HasFragment() && CurrentFragment != 0) {
                CurrentFragment--;
                return true;
            }
            return false;
        }
    }
}
