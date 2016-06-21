using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	// The audio source is where the audio file plays
	public AudioSource source;
	// Use this for initialization
	void Start () {

		// Get the audio source on the unit that is playing audio
		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Play the sound referenced in the factory for each sound
	public void PlayShootSound()
	{
		source.PlayOneShot (Controller.factory.shootSound);
	}

	public void PlaySwordSound()
	{
		source.PlayOneShot (Controller.factory.swordHitSound);
	}

	public void PlayPowerUpSound()
	{
		source.PlayOneShot (Controller.factory.powerUpSound);
	}

	public void PlayHitSound()
	{
		source.PlayOneShot (Controller.factory.hitSound);
	}

	public void PlayFlagCapSound()
	{
		source.PlayOneShot (Controller.factory.flagCapSound);
	}

	public void PlayBurnSound()
	{
		source.PlayOneShot (Controller.factory.burnSound);
	}

	public void PlayDeathSound()
	{
		source.PlayOneShot (Controller.factory.deathSound);
	}

	public void PlayArrowSound()
	{
		source.PlayOneShot (Controller.factory.archerAttack);
	}

	public void PlayLevelupSound()
	{
		source.PlayOneShot (Controller.factory.levelupSound);
	}

	public void PlayHuhSound()
	{
		source.PlayOneShot (Controller.factory.huhSound);
	}
}
