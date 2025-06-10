using System.Collections.Generic;
using UnityEngine;

public class ClownPotion : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Active = 1
    }
	private static readonly string[] sentenceTemplates =
	{
		"Hey {playerName}, did you just {transitiveVerb} that {noun}?!",
		"{playerName} {intransitiveVerb}s like a {adjective} circus act.",
		"I’ve seen {noun}s less {adjective} than you, {playerName}.",
		"Stop {intransitiveVerb} around, {playerName}! This isn’t a comedy show.",
		"Who needs a {noun}? {playerName} brings the slapstick.",
		"{playerName} could trip over a {noun} and still look {adjective}.",
		"Quit {transitiveVerb} my big red nose, {playerName}!",
		"That move was {intensifier} {adjective}, even for a clown.",
		"If pies {intransitiveVerb}, {playerName} would be a bakery.",
		"You juggle {noun}s about as well as a {adjective} banana, {playerName}.",
		"Why so serious, {playerName}? Let’s {transitiveVerb} a {noun}!",
		"Your brain’s more pie-filled than a {noun}, {playerName}.",
		"I’m {adverb} entertained by your {adjective} antics, {playerName}.",
		"Step right up, folks—watch {playerName} {intransitiveVerb} in circles!",
		"{playerName}, you’re the {intensifier} {adjective} joke of the night.",
		"Did someone order a {adjective}? Oh, it’s just {playerName}.",
		"Watch out! {playerName} just {transitiveVerb} the whoopee cushion!",
		"{playerName}, you’re wobblier than a {noun} on stilts.",
		"If slapstick was a sport, {playerName} would be champion!",
		"I’ve seen {noun}s less {adjective} than {playerName}.",
		"Honker! That’s the sound of {playerName}’s big red nose.",
		"Here comes {playerName}, clowning around again!",
		"{playerName} {intransitiveVerb}s worse than a deflated balloon.",
		"Your jokes are flat, {playerName}—like yesterday’s {noun}.",
		"Step right up! {playerName}’s silliness is {intensifier} {adjective}!",
		"If juggling ideas was a sport, {playerName} would drop ’em all!",
		"{playerName} tried tightrope walking… on a banana peel."
	};
	private static readonly string[] transitiveVerbs =
	{
		"tickle", "juggle", "honk at", "boop", "prank", "spook", "clown-chase", "slip on",
		"pie", "spring at", "squirt", "bounce on", "wiggle", "dangle before", "twirl at"
	};

	private static readonly string[] intransitiveVerbs =
	{
		"honk", "juggle", "wiggle", "boing", "clown-walk", "tumble", "prance",
		"flip", "bop", "flail"
	};

	private static readonly string[] adjectives =
	{
		"goofy", "wacky", "bouncy", "zany", "silly", "loopy", "nutty", "quirky",
		"bumbling", "clownish", "jolly", "bonkers", "whimsical", "bananas", "sprightly"
	};

	private static readonly string[] intensifiers =
	{
		"absurdly", "ridiculously", "outrageously", "hilariously", "uproariously",
		"comically", "bananas-level", "cartoonishly", "circus-grade", "bonkers-so"
	};

	private static readonly string[] nouns =
	{
		"balloon", "rubber chicken", "big shoe", "stilt", "squirt flower",
		"unicycle", "giant horn", "crazy wig", "red nose", "pie tray"
	};

	private static readonly string[] adverbs =
	{
		"hilariously", "clownishly", "wackily", "zestily", "buffoonishly",
		"sprightly", "boppingly", "slapstick-style", "boisterously", "twinkly"
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
        Color textColor = new Color(1f, 0.6f, 0.6f, 1f);
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