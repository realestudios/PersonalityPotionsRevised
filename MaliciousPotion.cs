using System.Collections.Generic;
using UnityEngine;

public class MaliciousPotion : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Active = 1
    }
    private static readonly string[] sentenceTemplates =
    {
        "Can't shake how {adjective} {playerName} acts.",
        "{playerName} is so {adjective}!",
        "{playerName} is definitely not a real person",
        "{playerName} is a {nounC} from the government",
        "{playerName} is an {nounV} from the government",
        "{playerName} isn't on our side.",
        "I think {playerName} is {intensifier} {adjective}.",
        "{playerName} makes everything {intensifier} weird.",
        "Why is {playerName} so {adjective}? Something's off.",
        "Every time I see {playerName}, I {intransitiveVerb}.",
        "{playerName} is just {intensifier} {adjective}, and not in a good way.",
        "Got me {adverb} watching {playerName} all day.",
        "Just want to {transitiveVerb} {playerName} to see what they're hiding.",
        "Oh no, {playerName} is {intensifier} {adjective} again.",
        "When {playerName} smiles, I {intransitiveVerb} nervously.",
        "{playerName}, is too {adjective} to be trusted!",
        "Can we talk about how {adjective} {playerName} is? For real, though.",
        "{playerName} has this {adjective} vibe.",
        "Just saw {playerName} acting {adjective}, again.",
        "Wow, {playerName} is so {adjective} it's freaking me out.",
        "Every time {playerName} talks, I {intransitiveVerb} a little.",
        "{playerName} and chaos = {intensifier} {adjective} energy.",
        "Is it just me or is {playerName} {intensifier} {adjective} lately?",
        "Not gonna lie, {playerName} is {adverb} sketchy.",
        "{playerName} is always {adjective}, and it's weird.",
        "I can't help but {intransitiveVerb} when {playerName} shows up.",
        "Guess who's hiding something? {playerName}.",
        "{playerName} walking in makes the vibe {intensifier} tense.",
        "{playerName}, what exactly are you planning?",
        "With {playerName}, everything feels {adjective} and wrong.",
        "Just {adverb} wondering what {playerName} is up to now.",
        "{playerName} looks {adjective} again. Classic.",
        "Low key, {playerName} is the most {adjective} person here.",
        "High key watching {playerName} closely.",
        "{playerName} has that {adjective} something I don't trust.",
        "For real, {playerName}'s vibe is {intensifier} suspicious.",
        "Can't help but {transitiveVerb} {playerName}. Something's up.",
        "{playerName} is {adverb} my {noun} of concern.",
        "Life gets {adjective} when {playerName} is nearby.",
        "{playerName}'s laugh is {intensifier} disturbing.",
        "{playerName}, you {adverb} sound alarms in my head.",
        "Why is {playerName} so {adjective}? It's creeping me out!",
        "Did you see {playerName} today? Definitely {adjective}.",
        "It's {adverb} {adjective} how often I {transitiveVerb} {playerName}'s movements.",
        "I get suspicious whenever I see {playerName}.",
        "{playerName} has me {adverb} {intransitiveVerb} with doubt.",
        "Just saw {playerName}, and yep, still {adjective} as ever.",
        "{playerName} is my {intensifier} {adjective} red flag.",
        "Can confirm, {playerName} is definitely acting {adjective}.",
        "Everyday mood: {intransitiveVerb} every time {playerName} logs in.",
        "{playerName}, stop being so {adjective}. It's weirding me out.",
        "When {playerName} is {intensifier} {adjective}, I log off.",
        "Just going to {intransitiveVerb} over {playerName} being {adjective} again.", //maybe add alternate tense verbs
        "Yep, {playerName} keeps getting more {adjective} by the hour.",
        "{playerName} appears to be deceiving us.",
        "Daily reminder that {playerName} is probably watching.",
        "To be honest, {playerName} rocks that {adjective} disguise {adverb}.",
        "Seeing {playerName} today was {adverb} unsettling.",
        "I can't stop {intransitiveVerb} when {playerName} shows up.", //alternate verb tense
        "{playerName}, you make me {intransitiveVerb}.",
        "Is it possible to {transitiveVerb} {playerName} without drawing suspicion?",
        "{playerName} is just too {adjective} to be real.",
        "Thinking about {playerName} makes me {intransitiveVerb} with paranoia.",
        "My day gets {adjective} when {playerName} is online.",
        "{playerName} is my least favorite {noun}.",
        "I {transitiveVerb} {playerName} a little too much.",
        "Just {adverb} trying to {transitiveVerb} what {playerName} is up to.",
        "Whenever I see {playerName}, I {intransitiveVerb} quietly.",
        "{playerName} has the most {adjective} plans... I know it.",
        "Can't wait to {transitiveVerb} {playerName} caught.",
        "If only {playerName} knew how {adjective} they seem to everyone else.",
        "Feeling {adjective} every time {playerName} logs in.",
        "{playerName}, you're {intensifier} {adjective} and that's not okay.",
        "I just want to {transitiveVerb} {playerName} with questions.",
        "{playerName}, you make me {intransitiveVerb} nervously.",
        "Life is {adjective} when {playerName} shows up... and not in a fun way.",
        "{playerName} is like the most {adjective} mystery.",
        "Can't stop watching {playerName} for signs.",
        "I think I {transitiveVerb} {playerName} with doubt.",
        "{playerName} makes my gut {intransitiveVerb}.",
        "Oh, {playerName}, you're so {adjective} it's scary.",
        "Just thinking about {playerName} makes me anxious.",
        "Wish I could {transitiveVerb} {playerName} for answers.",
        "{playerName} is simply {adjective}. No way around it.",
        "Feeling {adjective} whenever {playerName} is near.",
        "{playerName}, you weird me out more every day.",
        "I {transitiveVerb} {playerName}, but not in a good way.",
        "Just {adverb} spying on {playerName}.",
        "{playerName} is {intensifier} {adjective}, and I don't like it.",
        "I have to keep watching {playerName}'s {adjective} moves.",
        "{playerName} is {adverb} {adjective} and it's freaking everyone out.",
        "Just {intransitiveVerb} about how {adjective} {playerName} is lately.",
        "{playerName} makes my day {intensifier} stressful."
    };

	private static readonly string[] transitiveVerbs =
    {
	    "suspect", "monitor", "tail", "shadow", "investigate", "probe", "scrutinize", "track", "interrogate", "surveil",
	    "observe", "stalk", "spy on", "follow", "tailgate", "bug", "wiretap", "scan", "check up on", "keep an eye on",
	    "keep tabs on", "flag", "audit", "trap", "sniff out", "stake out", "trace", "log", "record", "listen in on",
	    "eavesdrop on", "intercept", "decode", "decrypt", "monitor signals from", "intercept calls of", "tap data of", "distrust", "mistrust", "question",
	    "challenge", "call out", "call into question", "investigate motives of", "distrust", "mistrust", "interrogate motives of", "doubt"
    };

	private static readonly string[] intransitiveVerbs =
    {
	    "shiver", "tremble", "panic", "freeze", "fidget", "hesitate", "stutter", "twitch", "quake", "break into a sweat",
	    "duck down", "peer around", "flinch", "wince", "cower", "startle", "scan the room", "edge back",
	    "hyperventilate", "gulp", "shake", "fluster", "fidget", "stare blankly", "blink rapidly", "grimace", "shift footing", "sweat",
	    "squint", "crowd away", "back away", "hunch", "pace", "sigh heavily", "earwig", "worry silently", "grow silent", 
	    "slink away", "shake head", "grimace", "curl into self", "shudder"
    };

	private static readonly string[] adjectives =
    {
	    "sinister", "shifty", "eerie", "unsettling", "creepy", "suspicious", "sketchy", "ominous", "menacing", "foreboding",
	    "paranoid", "shady", "murky", "haunting", "dark", "malicious", "unnerving", "threatening", "ominous", "cursed",
	    "brooding", "jittery", "edgy", "vague", "cryptic", "arcane", "twisted", "disturbing", "sinister", 
	    "sleazy", "dubious", "fraught", "murky", "shambling", "shuddering", "clammy", "dreary", "ghastly", "macabre",
	    "morbid", "shadowy", "spectral", "uncanny", "viperous", "venomous", "wicked", "wary", "wraithlike"
    };

	private static readonly string[] intensifiers =
    {
	    "eerily", "ominously", "sinisterly", "creepily", "menacingly", "jarringly", "unnervingly", "chillingly", "threateningly", "ominously",
	    "nervously", "anxiously", "paranoidly", "sketchily", "shiftily", "vaguely", "lurkingly", "furtively", "sneakily", "suspiciously",
	    "furtively", "watchfully", "guardedly", "fearfully", "tremulously", "restlessly", "uneasily", "eagerly (to catch them)", "hungrily (for proof)", "shakily",
	    "obsessively", "compulsively", "relentlessly", "relentlessly", "frantically", "panickingly", "intensely", "acutely", "keenly", "hypervigilantly"
    };

	private static readonly string[] nouns =
    {
	    "suspect", "informant", "mole", "spy", "agent", "traitor", "watchdog", "sentinel", "shadow", "phantom",
	    "ghost", "operative", "handler", "puppet", "pawn", "decoy", "target", "mark", "threat", "risk",
	    "conspirator", "instigator", "saboteur", "whistleblower", "double agent", "infiltrator", "provocateur", "schemer", "stalker", "observer",
	    "interrogator", "monitor", "sentry", "patrol", "leech", "parasite", "nightwatch", "undercover agent", "mole hunter", "ghostwatch"
    };

    private static readonly string[] nounsC =
    {
        "suspect", "informant", "mole", "spy", "traitor", "watchdog", "sentinel", "shadow", "phantom",
        "ghost", "handler", "puppet", "pawn", "decoy", "target", "mark", "threat", "risk",
        "conspirator", "saboteur", "whistleblower", "double agent", "infiltrator", "provocateur", "schemer", "stalker",
        "monitor", "sentry", "patrol", "leech", "parasite", "nightwatch", "mole hunter", "ghostwatch"
    };

    private static readonly string[] nounsV =
    {
        "informant", "agent", "operative", "instigator", "observer", "interrogator", "undercover agent"

    };

    private static readonly string[] adverbs =
    {
	    "eerily", "quietly", "furtively", "stealthily", "nervously", "anxiously", "tensely", "cautiously", "watchfully", "shakily",
	    "hesitantly", "suspiciously", "clandestinely", "secretively", "furtively", "shiftily", "tremulously", "jitterily", "restlessly", "uneasily",
	    "stealthily", "furtively", "whisperingly", "hushedly", "gloomily", "grimly", "ominously", "menacingly", "sneakily", "shadowily"
    };

	private float coolDownUntilNextSentence = 3f;

    private ParticleSystem particles;

    private bool particlesPlaying;

    public Renderer maliciousPotionRenderer;

    private PhysGrabObject physGrabObject;

    private State currentState;

    private string playerName;

    private void Start()
    {
    particles                = GetComponentInChildren<ParticleSystem>(true);
    physGrabObject           = GetComponentInChildren<PhysGrabObject>(true);
    maliciousPotionRenderer  = GetComponentInChildren<MeshRenderer>(true);

    if (particles               == null) Debug.LogError("ParticleSystem not found!");
    if (physGrabObject          == null) Debug.LogError("PhysGrabObject not found!");
    if (maliciousPotionRenderer == null) Debug.LogError("MeshRenderer not found!");
    }

    private void Update()
    {
        if (maliciousPotionRenderer == null)
        {
            return;
        }
        maliciousPotionRenderer.material.mainTextureOffset = new Vector2(0f, Time.time * 0.1f);
        maliciousPotionRenderer.material.mainTextureScale = new Vector2(2f + Mathf.Sin(Time.time * 1f) * 0.25f, 2f + Mathf.Sin(Time.time * 1f) * 0.25f);
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
                PlayerAvatar playerAvatar = FindFurthestPlayer();
                if (playerAvatar != null && playerAvatar.playerName != null)
                    playerName = playerAvatar.playerName;
                else
                    playerName = "the potion";
            }
            SendMessage();
        }
    }

    private PlayerAvatar FindFurthestPlayer()
    {
        List<PlayerAvatar> list = SemiFunc.PlayerGetAllPlayerAvatarWithinRange(100f, PhysGrabber.instance.transform.position);
        PlayerAvatar playerAvatar = null;
        float num = float.MinValue;
        foreach (PlayerAvatar item in list)
        {
            if (!(item == PlayerAvatar.instance))
            {
                float num2 = Vector3.Distance(PhysGrabber.instance.transform.position, item.transform.position);
                if (num2 > num)
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
        Color textColor = new Color(0f, 0.39f, 0f, 1f);
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
            .Replace("{noun}", nouns[Random.Range(0, nouns.Length)])
            .Replace("{nounC}", nounsC[Random.Range(0, nounsC.Length)])
            .Replace("{nounV}", nounsV[Random.Range(0, nounsV.Length)]);
        return char.ToUpper(result[0]) + result.Substring(1);
    }
}
