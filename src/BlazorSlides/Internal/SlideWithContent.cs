using System.Collections.Generic;
using System.Linq;
using System;

namespace BlazorSlides.Internal
{
  internal abstract class SlideWithContent
  {
    public List<IContent> Content { get; set; }

    public bool HasFragment() {
      return Content.Any(content => content is FragmentContent);
    }

    public int FragmentCount() {
      return Content.FindAll(content => content is FragmentContent).Count();
    }

    public int CurrentFragment { get; private set; } = 0;

    public bool NextFragment() {
      if (HasFragment() && CurrentFragment <= FragmentCount()) {
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
    
    public IEnumerable<FragmentContext> Contents {
      get {
        int i = 0;
        foreach (var item in Content)
        {
          if(item is FragmentContent) {
            yield return new FragmentContext { Content = item, IsCurrent = CurrentFragment == i + 1, IsPast = CurrentFragment > i + 1 };
            i++;
          } else {
            yield return new FragmentContext { Content = item };
          }
        }
      }
    }
  }
}
