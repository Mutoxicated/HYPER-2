using UnityEngine;
using UnityEngine.Events;

public class Interval : MonoBehaviour
{
    [SerializeField] private bool useUnscaledTime;
    [SerializeField] private bool dontResetTime;
    [SerializeField, Range(0, 100)] private int chance = 100;
    [SerializeField] private float interval;
    [SerializeField] public UnityEvent onInterval;
    [SerializeField] private bool destroyUponEvent;

    [HideInInspector] public float t = 0f;
    [HideInInspector] public float time = 0f;
    [HideInInspector] public bool isPlaying;
    private float deltaTime;

    public virtual float Rate {
        get { return interval; }
    }

    public void ResetEarly()//this is stupid but oh well lol
    {
        ResetInterval();
    }

    public void ResetEventless()
    {
        time = 0f;
        t = 0f;
    }

    private void ResetInterval()
    {
        if (Random.Range(0, 100) <= chance)
            onInterval.Invoke();
        if (destroyUponEvent)
            Destroy(gameObject);
        if (!dontResetTime){
            time = 0f;
            t = 0f;
        }
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Resume()
    {
        isPlaying = true;
    }

    public void ManipulateT(float t)
    {
        this.t = t;
    }

    public float GetInterval(){
        return Rate;
    }

    public void ChangeInterval(float interval)
    {
        this.interval = interval;
    }

    public void Enable() {
        enabled = true;
    }

    public void AddEvent(UnityAction action) {
        onInterval.AddListener(action);
    }

    public void RemoveEvent(UnityAction action) {
        onInterval.RemoveListener(action);
    }

    public void Disable() {
        ResetEventless();
        enabled = false;
    }

    private void OnEnable()
    {
        //Debug.Log(gameObject.name);
        time = 0f;
        t = 0f;
        isPlaying = true;
    }

    private void Update()
    {
        if (!isPlaying)
            return;
        if (useUnscaledTime){
            deltaTime = Time.unscaledDeltaTime;
        }else{
            deltaTime = Time.deltaTime;
        }
        time += deltaTime;
        //Debug.Log(time);
        if (time >= Rate)
        {
            ResetInterval();
        }
        if (time != 0f)
            t = time / Rate;
        else t = 0f;
    }
}