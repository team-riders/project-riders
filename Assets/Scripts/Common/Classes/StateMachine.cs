using System;
using System.Collections.Generic;
using System.Linq;

public class StateTransition
{
    List<Func<bool>> conditions = new();
    public int Priority;

    public void AddCondition(Func<bool> condition)
    {
        conditions.Add(condition);
    }

    public List<Func<bool>> GetConditions()
    {
        return conditions;
    }
}

public class State
{
    public string Name { get; set; }
    public bool JustEntered { get; set; }

    public State(string Name)
    {
        this.Name = Name;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}

public class StateMachine<T> where T : State
{
    readonly List<T> states;
    Dictionary<T, Dictionary<T, StateTransition>> stateTransitions;
    public T CurrentState { get; protected set; }

    public StateMachine(List<T> states)
    {
        this.states = states;
        CurrentState = states[0];

        stateTransitions = new();
    }

    public void AddTransition(T from, T to, Func<bool> condition, int priority = 0)
    {
        if (!stateTransitions.ContainsKey(from))
        {
            stateTransitions[from] = new();
        }

        if (!stateTransitions[from].ContainsKey(to))
        {
            stateTransitions[from][to] = new StateTransition
            {
                Priority = priority
            };
            stateTransitions[from][to].AddCondition(condition);
        }
        else
        {
            stateTransitions[from][to].AddCondition(condition);
            if (priority > stateTransitions[from][to].Priority)
            {
                stateTransitions[from][to].Priority = priority;
            }
        }
    }

    public Dictionary<T, StateTransition> GetTransitionsForState(T state)
    {
        if (stateTransitions.ContainsKey(state))
        {
            return stateTransitions[state];
        }
        return new Dictionary<T, StateTransition>();
    }

    public virtual void Update()
    {
        if (CurrentState == null) return;

        // Exit condition
        List<T> validTransitions = GetValidTransitions();
        if (validTransitions.Count > 0)
        {
            Transition(validTransitions[0]);
        }
        else if (CurrentState.JustEntered)
        {
            CurrentState.JustEntered = false;
            CurrentState.OnEnter();
        }
        else
        {
            CurrentState.OnUpdate();
        }
    }

    protected List<T> GetValidTransitions()
    {
        Dictionary<T, StateTransition> transitions = GetTransitionsForState(CurrentState);

        List<T> validStates = transitions.Where((t) => t.Value.GetConditions().All(condition => condition.Invoke()))
        .OrderByDescending(t => t.Value.Priority)
        .Select(t => t.Key)
        .ToList();

        return validStates;
    }

    public T GetStateByName(string name)
    {
        foreach (var state in states)
        {
            if (state.Name == name) return state;
        }
        return null;
    }

    protected void Transition(T newState)
    {
        if (!states.Contains(newState)) return;
        if (CurrentState == newState) return;

        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.JustEntered = true;
    }

    public void ForceTransition(T newState)
    {
        if (!states.Contains(newState)) return;
        if (CurrentState == newState) return;

        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState.JustEntered = true;
    }
}