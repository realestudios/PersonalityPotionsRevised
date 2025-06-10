using System.Collections.Generic;
using UnityEngine;

public class BrilliancePotion : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Active = 1
    }
	private static readonly string[] sentenceTemplates =
	{
		"{playerName} solves {noun}s with {intensifier} {adjective}.",
		"{playerName} {transitiveVerb}s problems {adverb}.",
		"{playerName} is {intensifier} {adjective}.",
		"{playerName} {intransitiveVerb}s past every {noun}.",
		"A true {noun}, {playerName} shines {adverb}.",
		"{playerName} cracks {noun}s with {intensifier} flair.",
		"Nothing stops {playerName}—{intransitiveVerb} and conquer.",
		"{playerName} {transitiveVerb}s tasks {adverb}.",
		"{playerName}'s mind is {intensifier} {adjective}.",
		"{playerName} masters {noun}s {adverb}.",
		"{playerName} {transitiveVerb}s challenges {adverb}.",
		"{playerName} thinks {intensifier} {adjective} thoughts.",
		"{playerName} makes {noun}s look {adverb}.",
		"{playerName} always {intransitiveVerb}s ahead.",
		"{playerName} solves puzzles {adverb}.",
		"{playerName} runs on {intensifier} {adjective} logic.",
		"{playerName} is a {noun} at work.",
		"{playerName} learns {adverb} and grows {intensifier}.",
		"{playerName} {transitiveVerb}s complexity with ease.",
		"{playerName} stands out {adverb} as a {adjective} {noun}."
	};
	private static readonly string[] transitiveVerbs =
	{
		"solve", "analyze", "decode", "decrypt", "untangle", "engineer", "architect", "formulate",
		"orchestrate", "spearhead", "envision", "design", "optimize", "streamline", "mastermind",
		"break down", "map out", "synthesize", "model", "calculate", "forecast", "predict", "interpret"
	};

	private static readonly string[] intransitiveVerbs =
	{
		"shine", "excel", "thrive", "soar", "flourish", "sparkle", "radiate", "stand out",
		"outperform", "dominate", "blink (in amazement)", "beam (with insight)", "glow", "gleam"
	};

	private static readonly string[] adjectives =
	{
		"brilliant", "insightful", "sharp", "astute", "perspicacious", "keen", "gifted",
		"ingenious", "savvy", "visionary", "masterful", "prodigious", "cerebral", "erudite",
		"lucid", "nimble-minded", "quick-witted", "sage-like", "preternatural"
	};

	private static readonly string[] intensifiers =
	{
		"exceptionally", "remarkably", "undeniably", "astoundingly", "profoundly",
		"outstandingly", "strikingly", "supremely", "incomparably", "tremendously",
		"unquestionably", "unusually", "supremely", "mind-bogglingly"
	};

	private static readonly string[] nouns =
	{
		"genius", "prodigy", "visionary", "luminary", "wizard", "maestro", "sage",
		"thinker", "oracle", "whiz", "trailblazer", "innovator", "architect", "strategist"
	};

	private static readonly string[] adverbs =
	{
		"brilliantly", "effortlessly", "flawlessly", "seamlessly", "masterfully",
		"gracefully", "keenly", "cunningly", "precisely", "adeptly", "swiftly",
		"decisively", "inspiredly"
	};

	private float coolDownUntilNextSentence = 3f;

    private ParticleSystem particles;

    private bool particlesPlaying;

    public Renderer BrilliancePotionRenderer;

    private PhysGrabObject physGrabObject;

    private State currentState;

    private string playerName;

    private void Start()
    {
    particles                = GetComponentInChildren<ParticleSystem>(true);
    physGrabObject           = GetComponentInChildren<PhysGrabObject>(true);
    BrilliancePotionRenderer  = GetComponentInChildren<MeshRenderer>(true);

    if (particles               == null) Debug.LogError("ParticleSystem not found!");
    if (physGrabObject          == null) Debug.LogError("PhysGrabObject not found!");
    if (BrilliancePotionRenderer == null) Debug.LogError("MeshRenderer not found!");
    }

    private void Update()
    {
        if (BrilliancePotionRenderer == null)
        {
            return;
        }
        BrilliancePotionRenderer.material.mainTextureOffset = new Vector2(0f, Time.time * 0.1f);
        BrilliancePotionRenderer.material.mainTextureScale = new Vector2(2f + Mathf.Sin(Time.time * 1f) * 0.25f, 2f + Mathf.Sin(Time.time * 1f) * 0.25f);
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
                playerName = "the potion";
            }
            else
            {
                PlayerAvatar playerAvatar = FindClosestPlayer();
                if (playerAvatar != null && playerAvatar.playerName != null)
                    playerName = playerAvatar.playerName;
                else
                    playerName = "the potion";
            }
        	SendMessage();
        }
    }

    private PlayerAvatar FindClosestPlayer()
    {
        List<PlayerAvatar> list = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(10f, PhysGrabber.instance.transform.position);
        PlayerAvatar playerAvatar = null;
        float num = float.MinValue;
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
        Color textColor = new Color(0.2f, 0.9f, 1f, 1f);
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
            .Replace("{intransitiveVerb}", intransitiveVerbs[Random.Range(0, intransitiveVerbs.Length)])
            .Replace("{adjective}", adjectives[Random.Range(0, adjectives.Length)])
            .Replace("{intensifier}", intensifiers[Random.Range(0, intensifiers.Length)])
            .Replace("{adverb}", adverbs[Random.Range(0, adverbs.Length)])
            .Replace("{noun}", nouns[Random.Range(0, nouns.Length)]);
        return char.ToUpper(result[0]) + result.Substring(1);
    }
}