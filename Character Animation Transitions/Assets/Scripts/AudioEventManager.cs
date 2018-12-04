using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{

    public AudioClip boxAudio;
    public AudioClip bearAudio;
    public AudioClip playerLandsAudio;
    public AudioClip[] footStepAudio;
    public AudioClip jumpAudio;
    public AudioClip fallAudio;
    public AudioClip playerDeathAudio;

    private UnityAction<Vector3> boxCollisionEventListener;
    private UnityAction<Vector3> bearCollisionEventListener;
    private UnityAction<Vector3> playerLandsEventListener;
    private UnityAction<Vector3, int> footStepEventListener;
    private UnityAction<Vector3> jumpEventListener;
    private UnityAction<Vector3> fallEventListener;
    private UnityAction<Vector3> playerDeathEventListener;

    void Awake()
    {
        boxCollisionEventListener = new UnityAction<Vector3>(boxCollisionEventHandler);
        bearCollisionEventListener = new UnityAction<Vector3>(bearCollisionEventHandler);
        playerLandsEventListener = new UnityAction<Vector3>(playerLandsEventHandler);
        footStepEventListener = new UnityAction<Vector3, int>(footStepEventHandler);
        jumpEventListener = new UnityAction<Vector3>(jumpEventHandler);
        fallEventListener = new UnityAction<Vector3>(fallEventHandler);
        playerDeathEventListener = new UnityAction<Vector3>(playerDeathEventHandler);
    }

    // Use this for initialization
    void Start()
    {       			
    }

    void OnEnable()
    {
        EventManager.StartListening<BoxCollisionEvent, Vector3>(boxCollisionEventListener);
        EventManager.StartListening<BearCollisionEvent, Vector3>(bearCollisionEventListener);
        EventManager.StartListening<PlayerLandsEvent, Vector3>(playerLandsEventListener);
        EventManager.StartListening<FootStepEvent, Vector3, int>(footStepEventListener);
        EventManager.StartListening<JumpEvent, Vector3>(jumpEventListener);
        EventManager.StartListening<FallEvent, Vector3>(fallEventListener);
        EventManager.StartListening<PlayerDeathEvent, Vector3>(playerDeathEventListener);
    }

    void OnDisable()
    {
        EventManager.StopListening<BoxCollisionEvent, Vector3>(boxCollisionEventListener);
        EventManager.StopListening<BearCollisionEvent, Vector3>(bearCollisionEventListener);
        EventManager.StopListening<PlayerLandsEvent, Vector3>(playerLandsEventListener);
        EventManager.StopListening<FootStepEvent, Vector3, int>(footStepEventListener);
        EventManager.StopListening<JumpEvent, Vector3>(jumpEventListener);
        EventManager.StopListening<FallEvent, Vector3>(fallEventListener);
        EventManager.StopListening<PlayerDeathEvent, Vector3>(playerDeathEventListener);
    }
	
    // Update is called once per frame
    void Update()
    {
    }

    void boxCollisionEventHandler(Vector3 worldPos)
    {
        float volume = Random.Range(0.3f, 1.0f);
        AudioSource.PlayClipAtPoint(this.boxAudio, worldPos, volume);
    }

    void bearCollisionEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.bearAudio, worldPos);
    }

    void playerLandsEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.playerLandsAudio, worldPos);
    }

    void footStepEventHandler(Vector3 worldPos, int index)
    {
        float volume = Random.Range(0.3f, 1.0f);
        AudioSource.PlayClipAtPoint(this.footStepAudio[index], worldPos, volume);
    }

    void jumpEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.jumpAudio, worldPos);
    }

    void fallEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.fallAudio, worldPos);
    }

    void playerDeathEventHandler(Vector3 worldPos)
    {
        AudioSource.PlayClipAtPoint(this.playerDeathAudio, worldPos);
    }
}