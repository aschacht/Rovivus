//EBunny_AIController: Handles Both the electronic bunny's AI and animations
//Since we don't need to be checking for the player's input or actions, playing the proper animations is much more straightforward and can be handled
//in the same script.

//------------------------------------------------------------------
var walkSpeed = 3.0;
var rotateSpeed = 30.0;
var attackSpeed = 8.0;
var attackRotateSpeed = 80.0;

var directionTraveltime = 2.0;
var idleTime = 1.5;
var attackDistance = 15.0;

var damage = 1;

var attackRadius = 2.5;
var viewAngle = 20.0;
var attackTurnTime = 1.0;
var attackPosition = new Vector3 (0, 1, 0);


//------------------------------------------------------------------

private var isAttacking = false;
private var lastAttackTime = 0.0;
private var nextPauseTime = 0.0;
private var distanceToPlayer;
private var timeToNewDirection = 0.0;

var target : Transform;

// Cache the controller
private var characterController : CharacterController;
characterController = GetComponent(CharacterController);


//------------------------------------------------------------------
function Start ()
{
	if (!target)
		target = GameObject.FindWithTag("Player").transform;
	
	// Setup animations-----------------------------------------------------------------
	animation.wrapMode = WrapMode.Loop;
	animation["EBunny_Death"].wrapMode = WrapMode.Once;

	animation["EBunny_Attack"].layer = 1;
	animation["EBunny_Hit"].layer = 3;
	animation["EBunny_Death"].layer = 5;
		
	// Rather than placing this in an update function, we can start the AI's behavior now and 
	//use coroutines to handle the changes
	yield WaitForSeconds(idleTime);
	
	while (true)	
	{
		// Idle around and wait for the player
		yield Idle();

		// Player has been located, prepare for the attack.
		yield Attack();
	}
}


function Idle ()
{
	// Walk around and pause in random directions unless the player is within range
	while (true)
	{
		// Find a new direction to move
		if(Time.time > timeToNewDirection)
		{
			yield WaitForSeconds(idleTime);
			
			if(Random.value > 0.5)
				transform.Rotate(Vector3(0,5,0), rotateSpeed);
			else
				transform.Rotate(Vector3(0,-5,0),rotateSpeed);
				
			timeToNewDirection = Time.time + directionTraveltime;
		}
		
		var walkForward = transform.TransformDirection(Vector3.forward);
		characterController.SimpleMove(walkForward * walkSpeed);
		
		distanceToPlayer  = transform.position - target.position;
			
		//We found the player!  Stop wasting time and go after him
		if (distanceToPlayer.magnitude < attackDistance)
			return;
		
		yield;
	}
} 

function Attack ()
{
	isAttacking = true;
	animation.Play("EBunny_Attack");
	
	// We need to turn to face the player now that he's in range.
	var angle  = 0.0;
	var time  = 0.0;
	var direction : Vector3;
	while (angle > viewAngle || time < attackTurnTime)
	{
		time += Time.deltaTime;
		angle = Mathf.Abs(FacePlayer(target.position, attackRotateSpeed));
		move = Mathf.Clamp01((90 - angle) / 90);
		
		// depending on the angle, start moving
		animation["EBunny_Attack"].weight = animation["EBunny_Attack"].speed = move;
		direction = transform.TransformDirection(Vector3.forward * attackSpeed * move);
		characterController.SimpleMove(direction);
		
		yield;
	}
	
	// attack if can see player
	var lostSight = false;
	while (!lostSight)
	{
		angle = FacePlayer(target.position, attackRotateSpeed);
			
		// Check to ensure that the target is within the Bunny's eyesight
		if (Mathf.Abs(angle) > viewAngle)
			lostSight = true;
			
		// If bunny loses site of the player, he jumps out of here.
		if (lostSight)
			break;
		
		//Check to see if we're close enough to the player to bite 'em.
		var location = transform.TransformPoint(attackPosition) - target.position;
		if(Time.time > lastAttackTime + 1.0 && location.magnitude < attackRadius)
		{
			// deal damage
			target.SendMessage("ApplyDamage", damage);
			
			lastAttackTime = Time.time;
		}

		if(location.magnitude > attackRadius)
			break;
			
		// Check to make sure our current direction didn't collide us with something
		if (characterController.velocity.magnitude < attackSpeed * 0.3)
			break;
		
		// yield for one frame
		yield;
	}

	isAttacking = false;
	
}

function FacePlayer(targetLocation : Vector3, rotateSpeed : float) : float
{
	// Find the relative place in the world where the player is located
	var relativeLocation = transform.InverseTransformPoint(targetLocation);
	var angle = Mathf.Atan2 (relativeLocation.x, relativeLocation.z) * Mathf.Rad2Deg;
	
	// Clamp it with the max rotation speed so he doesn't move too fast
	var maxRotation = rotateSpeed * Time.deltaTime;
	var clampedAngle = Mathf.Clamp(angle, -maxRotation, maxRotation);
	
	// Rotate
	transform.Rotate(0, clampedAngle, 0);
	// Return the current angle
	return angle;
}

@script AddComponentMenu("Enemies/Bunny'sAIController")