public class Stopwatch
{
    bool isRunning = false;

    float elapsedTime = 0f;

    public float Now => elapsedTime;

    public void Start()
    {
        if (!isRunning)
        {
            isRunning = true;
            elapsedTime = 0f;
        }
    }

    public void Update(float deltaTime)
    {
        if (isRunning)
        {
            elapsedTime += deltaTime;
        }
    }

    public void Pause()
    {
        if (isRunning)
        {
            isRunning = false;
        }
    }

    public void Resume()
    {
        if (!isRunning)
        {
            isRunning = true;
        }
    }

    public void Reset()
    {
        elapsedTime = 0f;
        isRunning = false;
    }
}