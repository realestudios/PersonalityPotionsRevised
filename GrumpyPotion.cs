using System.Collections.Generic;
using UnityEngine;

public class GrumpyPotion : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Active = 1
    }
	private static readonly string[] sentenceTemplates =
{
    "{playerName} {intransitiveVerb}s about every little {noun}.",
    "I’m {intensifier} {adjective} at {playerName} today.",
    "{playerName} {transitiveVerb} my patience {adverb}.",
    "Good grief, {playerName} is {intensifier} {adjective}.",
    "Ugh, {playerName} and their {noun}s again.",
    "Stop being such a {noun}, {playerName}.",
    "Can’t {intransitiveVerb} another {noun} from {playerName}.",
    "{playerName} makes me {intransitiveVerb} {adverb}.",
    "I’ll {transitiveVerb} {playerName} for this {noun}.",
    "{playerName} is a {adjective} {noun}, honestly.",
    "{playerName} moves {adverb} what a {noun}.",
    "Why is {playerName} so {adjective}? It’s {intensifier} annoying.",
    "This whole place feels like a {noun}.",
    "Why is everything so {intensifier} {adjective} today?",
    "I’m {intransitiveVerb} at life for this {noun}",
        "{playerName} {intransitiveVerb}s louder than a {noun}.",
    "Honestly, {playerName}'s {noun}s are {intensifier} {adjective}.",
    "If I hear another {noun} from {playerName}, I'll {transitiveVerb} them myself.",
    "Todays weather is {intensifier} dreadful.",
    "I have to deal with you {adjective} people again..",
    "{intensifier} day today, I wish {playerName} stayed home.",
    "I hope {playerName}'s day is {intensifier} ruined."
    };
private static readonly string[] transitiveVerbs =
{
    "grumble at", "complain about", "scoff at", "growl at", "mutter about",
    "grudge against", "snarl at", "grumble under", "belabor", "harangue",
    "gripe about", "berate", "chide", "rail against", "bemoan", "rant about",
    "snap at",
};

private static readonly string[] intransitiveVerbs =
{
    "grumble", "sulk", "pout", "snarl", "frown", "scowl", "brood", "cringe",
    "grouse", "mutter", "bluster", "huff", "mope"
};

private static readonly string[] adjectives =
{
    "grumpy", "cranky", "irritable", "surly", "cantankerous", "petulant", "gruff", "sour",
    "testy", "cross", "ornery", "peevish", "crabby", "prickly"
};

private static readonly string[] intensifiers =
{
    "absurdly", "ridiculously", "incredibly", "painfully", "miserably", "excessively",
    "deeply", "utterly", "bumblingly", "perpetually", "stubbornly",  "unbearably",
    "hideously", "embarrassingly", "agonizingly", "frustratingly", "vexingly"
};

private static readonly string[] nouns =
{
    "nuisance", "headache", "pest", "burden", "fuss", "thorn in my side", "time sink",
    "annoyance", "hassle", "mess", "pothole", "snag", "roadblock", "blight"
};
private static readonly string[] adverbs =
{
    "begrudgingly", "grudgingly", "reluctantly", "sourly", "crossly", "grumpily",
    "irritably", "huffily", "mopeyly", "grouchily", "snappishly", "stubbornly", "sullenly"
};
	private float coolDownUntilNextSentence = 3f;

    private ParticleSystem particles;

    private bool particlesPlaying;

    public Renderer GrumpyPotionRenderer;

    private PhysGrabObject physGrabObject;

    private State currentState;

    private string playerName;

    private void Start()
    {
    particles                = GetComponentInChildren<ParticleSystem>(true);
    physGrabObject           = GetComponentInChildren<PhysGrabObject>(true);
    GrumpyPotionRenderer  = GetComponentInChildren<MeshRenderer>(true);

    if (particles               == null) Debug.LogError("ParticleSystem not found!");
    if (physGrabObject          == null) Debug.LogError("PhysGrabObject not found!");
    if (GrumpyPotionRenderer == null) Debug.LogError("MeshRenderer not found!");
    }

    private void Update()
    {
        if (GrumpyPotionRenderer == null)
        {
            return;
        }
        GrumpyPotionRenderer.material.mainTextureOffset = new Vector2(0f, Time.time * 0.1f);
        GrumpyPotionRenderer.material.mainTextureScale = new Vector2(2f + Mathf.Sin(Time.time * 1f) * 0.25f, 2f + Mathf.Sin(Time.time * 1f) * 0.25f);
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
                Enemy Enemy = SemiFunc.EnemyGetNearest(PhysGrabber.instance.transform.position, 20f, false);
                if (Enemy != null && Enemy.EnemyParent != null)
                    playerName = Enemy.EnemyParent.enemyName;
                else
                    playerName = "the potion";
            }
            SendMessage();
        }
    }

    private void SendMessage()
    {
        string message = GenerateAffectionateSentence();
        Color textColor = new Color(0.8f, 0.1f, 0.1f, 1f);
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