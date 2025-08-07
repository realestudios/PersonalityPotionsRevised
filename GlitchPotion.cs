using System.Collections.Generic;
using UnityEngine;

public class GlitchPotion : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Active = 1
    }
	private static readonly string[] sentenceTemplates =
    {
        "{playerName} just caused a {noun} system integrity at risk!",
        "Warning : {playerName} is {adverb} {intransitiveVerbAlt}!",
        "{playerName} {transitiveVerbPast} my code with {intensifier} {adjective} force.",
        "Error 0xDEADBEEF: {playerName} is too {adjective}.",
        "Kernel panic: {playerName} overloads the CPU.",
        "{playerName}’s logic {intransitiveVerb}s in a {nounC}.",
        "{playerName}’s logic {intransitiveVerb}s in an {nounV}.",
        "Compiler says: {playerName} does not compute.",
        "{playerName} triggers segmentation fault {adverb}.",
        "StackOverflowException: {playerName} recursed infinitely!",
        "Boot sequence hijacked by {playerName}, {intensifier} {adjective}!",
        "Fatal error: {playerName} is {adverb} {adjective}.",
        "{playerName}’s code is a {nounC} waiting to happen.",
        "{playerName}’s code is an {nounV} waiting to happen.",
        "Unhandled exception at {playerName}.dll!",
        "{playerName} crashes before Hello World even starts.",
        "System logs: {playerName} is {intensifier} corrupting memory.",
        "ERROR double free detected on {adjective} {playerName}",
        "Missing parathesis at {playerName} line two hundred and eleven"
	};
	private static readonly string[] transitiveVerbs =
	{
		"segfault at", "overflow", "corrupt", "freeze on", "crash into", "null ref at",
		"stack overflow on", "index out of bounds on", "deadlock with"
	};
    private static readonly string[] transitiveVerbsPast =
    {
        "segfaulted", "overflowed", "corrupted", "frozen", "crashed",
        "deadlocked"
    };

	private static readonly string[] intransitiveVerbs =
	{
		"crash", "freeze", "glitch", "lag", "flicker", "stutter", "jit­ter", "loop", "halt"
	};

    private static readonly string[] intransitiveVerbsAlt =
    {
        "crashing", "freezing", "glitching", "lagging", "flickering", "stuttering",
        "jit­tering", "looping", "halting"
    };


	private static readonly string[] adjectives =
	{
		"glitchy", "corrupted", "broken", "unstable", "bugged", "frozen",
		"fragmented", "faulty", "janky", "laggy"
	};

	private static readonly string[] intensifiers =
	{
		"critically", "fatal level", "kernel panic grade", "utterly", "catastrophically",
		"painfully", "sporadically", "relentlessly", "infuriatingly", "randomly"
	};

	private static readonly string[] nouns =
	{
		"buffer overflow", "null pointer", "segfault", "stack overflow", "deadlock",
		"race condition", "memory leak", "bit flip", "core dump", "exception"
	};

    private static readonly string[] nounsC =
    {
        "buffer overflow", "null pointer", "segfault", "stack overflow", "deadlock",
        "race condition", "memory leak", "bit flip", "core dump"

    }:

    private static readonly string[] nounsV =
    {
        "exception"
    }:

	private static readonly string[] adverbs =
	{
		"erratically", "chaotically", "incessantly", "unpredictably", "violently",
		"haphazardly", "relentlessly", "painstakingly", "mercilessly", "spastically"
	};

	private float coolDownUntilNextSentence = 3f;

    private ParticleSystem particles;

    private bool particlesPlaying;

    public Renderer ClownPotionRenderer;

    private PhysGrabObject physGrabObject;

    private State currentState;

    private string playerName;

    private void Start()
    {
    particles                = GetComponentInChildren<ParticleSystem>(true);
    physGrabObject           = GetComponentInChildren<PhysGrabObject>(true);
    ClownPotionRenderer  = GetComponentInChildren<MeshRenderer>(true);

    if (particles               == null) Debug.LogError("ParticleSystem not found!");
    if (physGrabObject          == null) Debug.LogError("PhysGrabObject not found!");
    if (ClownPotionRenderer == null) Debug.LogError("MeshRenderer not found!");
    }

    private void Update()
    {
        if (ClownPotionRenderer == null)
        {
            return;
        }
        ClownPotionRenderer.material.mainTextureOffset = new Vector2(0f, Time.time * 0.1f);
        ClownPotionRenderer.material.mainTextureScale = new Vector2(2f + Mathf.Sin(Time.time * 1f) * 0.25f, 2f + Mathf.Sin(Time.time * 1f) * 0.25f);
        var trails = particles.trails;
        if (physGrabObject.grabbed)
        {
            if (!particlesPlaying)
            {
                particles.Play();
                trails.enabled = true;
                particlesPlaying = true;
            }
        }
        else if (particlesPlaying)
        {
            particles.Stop();
            trails.enabled = false;
            particlesPlaying = false;
        }
        if (SemiFunc.IsMultiplayer())
        {
            switch (currentState)
            {
                case State.Idle:
                    StateIdle();
                    break;
                case State.Active:
                    StateActive();
                    break;
            }
        }
    }

    private void StateIdle()
    {
        if (coolDownUntilNextSentence > 0f && physGrabObject.grabbed)
        {
            coolDownUntilNextSentence -= Time.deltaTime;
        }
        else
        {
            if (!PhysGrabber.instance || !PhysGrabber.instance.grabbed || !PhysGrabber.instance.grabbedPhysGrabObject
             || !(PhysGrabber.instance.grabbedPhysGrabObject == physGrabObject))
                return;
            if (!SemiFunc.IsMultiplayer())
            {
                playerName = PlayerAvatar.instance.playerName;
            }
            else
            {
                PlayerAvatar playerAvatar = FindClosestPlayer();
                if (playerAvatar != null && playerAvatar.playerName != null)
                    playerName = playerAvatar.playerName;
                else
                    playerName = PlayerAvatar.instance.playerName;
            }
			if (playerName == null)
				playerName = "null";
            SendMessage();
        }
    }

    private PlayerAvatar FindClosestPlayer()
    {
        List<PlayerAvatar> list = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(10f, PhysGrabber.instance.transform.position);
        PlayerAvatar playerAvatar = null;
        float num = float.MaxValue;
        foreach (PlayerAvatar item in list)
        {
            if (!(item == PlayerAvatar.instance))
            {
                float num2 = Vector3.Distance(PhysGrabber.instance.transform.position, item.transform.position);
                if (num2 < num)
                {
                    num = num2;
                    playerAvatar = item;
                }
            }
        }
        return (playerAvatar);
    }

    private void SendMessage()
    {
        string message = GenerateAffectionateSentence();
        Color textColor = new Color(1f, 1f, 0f, 1f);
        ChatManager.instance.PossessChatScheduleStart(10);
        ChatManager.instance.PossessChat(ChatManager.PossessChatID.LovePotion, message, 1f, textColor);
        ChatManager.instance.PossessChatScheduleEnd();
        currentState = State.Active;
        return;
    }

    private void StateActive()
    {
        if (PhysGrabber.instance.grabbed && (bool)PhysGrabber.instance.grabbedPhysGrabObject && PhysGrabber.instance.grabbedPhysGrabObject != physGrabObject)
        {
            currentState = State.Idle;
            coolDownUntilNextSentence = Random.Range(5f, 10f);
        }
        else if (!ChatManager.instance.StateIsPossessed())
        {
            currentState = State.Idle;
            coolDownUntilNextSentence = Random.Range(5f, 10f);
        }
    }
    private string GenerateAffectionateSentence()
    {
        string text = sentenceTemplates[Random.Range(0, sentenceTemplates.Length)];
        string result = text
            .Replace("{playerName}", playerName)
            .Replace("{transitiveVerb}", transitiveVerbs[Random.Range(0, transitiveVerbs.Length)])
            .Replace("{transitiveVerbPast}", transitiveVerbsPast[Random.Range(0, transitiveVerbsPast.Length)])
            .Replace("{intransitiveVerb}", intransitiveVerbs[Random.Range(0, intransitiveVerbs.Length)])
            .Replace("{intransitiveVerbAlt}", intransitiveVerbsAlt[Random.Range(0, intransitiveVerbsAlt.Length)])
            .Replace("{adjective}", adjectives[Random.Range(0, adjectives.Length)])
            .Replace("{intensifier}", intensifiers[Random.Range(0, intensifiers.Length)])
            .Replace("{adverb}", adverbs[Random.Range(0, adverbs.Length)])
            .Replace("{noun}", nouns[Random.Range(0, nouns.Length)]);
            .Replace("{nounC}", nounsC[Random.Range(0, nounsC.Length)]);
            .Replace("{nounV}", nounsV[Random.Range(0, nounsV.Length)]);
        return char.ToUpper(result[0]) + result.Substring(1);
    }
}
