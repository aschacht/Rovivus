//Enemy_RespawnPoint: attach to a GO in the scene to serve as a respawn point for enemies.  When the player walks into the 
//specified area, a new enemy will respawn.

//---------------------------------------------------------
var spawnRange = 40.0;	//Make sure it's big enough so that the enemy doesn't 'pop' into view
var enemy: GameObject;	

private var target : Transform;
private var currentEnemy : GameObject;   //only allow one enemy at a time to spawn
private var outsideRange = true;
private var distanceToPlayer;

//---------------------------------------------------------
function Start ()
{
	target = GameObject.FindWithTag("Player").transform;
}

function Update ()
{
	distanceToPlayer  = transform.position - target.position;

	// check to see if player encounters the respawn point.
	if (distanceToPlayer.magnitude < spawnRange)
	{	
		if (!currentEnemy)
		{
			currentEnemy = Instantiate(enemy, transform.position, transform.rotation);
		}
		
		// the player is now inside the respawn's range
		outsideRange = false;
	}
	
	// player is moving out of range, so get rid of the unnecessary enemy now
	else
	{	
		if (currentEnemy)
			Destroy(currentEnemy);	
		}
		outsideRange = true;
}

@script AddComponentMenu("Enemies/Respawn Point")