using System.Collections.Generic;

namespace Adapt
{
    public interface IObserver
    {
        public void update(Dictionary<string,object> data);
    }
}