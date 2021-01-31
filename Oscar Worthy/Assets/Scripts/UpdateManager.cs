using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Intended to be accessed through the static GameManager field. Not enforcing singleton due to small scope
public class UpdateManager
{
    List<IUpdateable> subscribers = new List<IUpdateable>();

    //private static readonly Object threadLock = new Object();

    //private static UpdateManager _instance;

    //public static UpdateManager Instance
    //{
    //    get
    //    {
    //        lock (threadLock)
    //        {
    //            if (_instance == null)
    //            {
    //                _instance = this;
    //            }
    //            return _instance;
    //        }
    //    }
    //}

    //// Referencing https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    ////  for a Unity implementation of a singleton pattern
    //public void Awake()
    //{
    //    if (_instance != null && _instance != this)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        _instance = this;
    //    }
    //}

    public UpdateManager() { }

    //private List<IUpdateable> subscribersToAdd = new List<IUpdateable>();
    //private List<IUpdateable> subscribersToRemove = new List<IUpdateable>();
    //public void Subscribe(IUpdateable updateable)
    //{
    //    if(!subscribers.Contains(updateable))
    //        subscribersToAdd.Add(updateable);
    //}

    //public void Unsubscribe(IUpdateable updateable)
    //{
    //    subscribersToRemove.Add(updateable);
    //}
    
    public void Subscribe(IUpdateable updateable)
    {
        subscribers.Add(updateable);
    }

    public void Unsubscribe(IUpdateable updateable)
    {
        subscribers.Remove(updateable);
    }

    // Helper list to avoid modifying while enumerating. Not ideal to copy the list each 
    //  frame but can't think of a cleaner solution at the moment
    private List<IUpdateable> subscribersCopy = new List<IUpdateable>();
    public void Update(float dt)
    {
        subscribersCopy.Clear();
        subscribersCopy.AddRange(subscribers);

        foreach(IUpdateable updateable in subscribersCopy)
        {
            updateable.Update(dt);
        }

    }
}
