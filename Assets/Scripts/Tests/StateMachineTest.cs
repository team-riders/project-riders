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

        state.AddTransition(state2, () => condition1);
        state.AddTransition(state3, () => condition2);

        state2.AddTransition(state, () => condition3);
        state2.AddTransition(state3, () => condition3, 2);
        state2.AddTransition(state3, () => condition1);

        state3.AddTransition(state, () => condition2);
        state3.AddTransition(state2, () => condition2, 2);
        state3.AddTransition(state2, () => condition1);

        stateMachine = new(new() { state, state2, state3 });
    }

    [SetUp]
    public void SetUp()
    {
        condition1 = false;
        condition2 = false;
        condition3 = false;
        stateMachine.ForceTransition(stateMachine.GetStateByName("TestState"));
    }

    // A Test behaves as an ordinary method


    [Test]
    public void T1_StateStaysTheSame()
    {
        stateMachine.Update();
        Assert.AreEqual("TestState", stateMachine.CurrentState.Name);
        stateMachine.Update();
        Assert.AreEqual("TestState", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T2_StateChangesOnce()
    {
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T3_StateChancesMidWay()
    {
        condition1 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState2", stateMachine.CurrentState.Name);
        condition3 = true;
        stateMachine.Update();
        Assert.AreEqual("TestState3", stateMachine.CurrentState.Name);
    }

    [Test]
    public void T4_TransitionBackToState1()
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
}
