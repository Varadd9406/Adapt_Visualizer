using System.Collections.Generic;

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
        private Data data;
        public StatePublisher()
        {
            observers = new List<IObserver>();
        }

        public void register(IObserver observer)
        {

        }
        public void unregister(IObserver observer)
        {

        }
        public void notifyObserver()
        {
            foreach (IObserver observer in observers)
            {
                observer.update(data);
            }
        }

        public void getState()
        {

        }



    }
}