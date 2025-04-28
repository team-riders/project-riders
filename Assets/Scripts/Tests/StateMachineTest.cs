using NUnit.Framework;

public class StateMachineTest
{
    StateMachine<State> stateMachine;

    bool condition1;
    bool condition2;
    bool condition3;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        State state = new("TestState");
        State state2 = new("TestState2");
        State state3 = new("TestState3");

        stateMachine = new(new() { state, state2, state3 });

        stateMachine.AddTransition(state, state2, () => condition1);
        stateMachine.AddTransition(state, state3, () => condition2);

        stateMachine.AddTransition(state2, state, () => condition3);
        stateMachine.AddTransition(state2, state3, () => condition3, 2);
        stateMachine.AddTransition(state2, state3, () => condition1);

        stateMachine.AddTransition(state3, state, () => condition2);
        stateMachine.AddTransition(state3, state2, () => condition2, 2);
        stateMachine.AddTransition(state3, state2, () => condition1);


    }

    [SetUp]
    public void SetUp()
    {
        condition1 = false;
        condition2 = false;
        condition3 = false;
        stateMachine.ForceTransition(stateMachine.GetStateByName("TestState"));
        stateMachine.CurrentState.JustEntered = false;
    }

    // A Test behaves as an ordinary method


    [Test]
    public void T1_StateMachine_ShouldStayTheSame()
    {
        stateMachine.Update();
        Assert.AreEqual("TestState", stateMachine.CurrentState.Name);
        stateMachine.Update();
        Assert.AreEqual("TestState", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T2_Condition1_ShouldTransitiontoState2()
    {
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T3_StateChancesMidWay_ShouldTransitionToState2Then3()
    {
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
        condition3 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState3", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T4_StateChanges_ShouldTransitionToState1ThenToState2()
    {
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
        condition1 = false;
        condition3 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T5_TransitionToState3AndBackToState2()
    {
        condition2 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState3", stateMachine.CurrentState.Name);
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T6_TransitionToState2AndInvokeOnEntry()
    {
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
    }
}
