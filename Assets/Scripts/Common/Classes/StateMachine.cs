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

    public Dictionary<State, StateTransition> Transitions { get; set; }

    public State(string Name)
    {
        this.Name = Name;
        Transitions = new Dictionary<State, StateTransition>();
    }

    public void AddTransition(State state, Func<bool> condition, int priority = 0)
    {
        if (!Transitions.ContainsKey(state))
        {
            Transitions[state] = new()
            {
                Priority = priority
            };
            Transitions[state].AddCondition(condition);
        }
        else
        {
            Transitions[state].AddCondition(condition);
            if (priority > Transitions[state].Priority)
            {
                Transitions[state].Priority = priority;
            }
        }
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}

public class StateMachine<T> where T : State
{
    readonly List<T> states;
    public T CurrentState { get; protected set; }

    public StateMachine(List<T> states)
    {
        this.states = states;
        CurrentState = states[0];
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
        List<T> validStates = validStates = CurrentState.Transitions
            .Where(t => t.Value.GetConditions().All(condition => condition.Invoke()))
            .OrderByDescending(t => t.Value.Priority)
            .Select(t => (T)t.Key)
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