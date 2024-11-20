using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;

namespace Adapt
{

    public interface IPublisher
    {
        public void register(IObserver observer);
        public void unregister(IObserver observer);
        public void notifyObserver();
    }

    public class StatePublisher : IPublisher
    {
        private List<IObserver> observers;
        private Dictionary<string,object> data;
        public StatePublisher()
        {
            observers = new List<IObserver>();
        }

        public void register(IObserver observer)
        {
            observers.Add(observer);
            UnityEngine.Debug.Log("registered");
        }
        public void unregister(IObserver observer)
        {

        }
        public void notifyObserver()
        {

            foreach (IObserver observer in observers)
            {
                if(data!=null)
                {

                    observer.update(data);
                }

            }
        }

        public async Task getState()
        {
            data = await RestClient.GetJsonAsDictionary("http://localhost:5000/temperatures");
        }



    }
}