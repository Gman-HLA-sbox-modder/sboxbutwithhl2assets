using Sandbox;
using System;

[Library( "vehicle_apc", Title = "Combine APC", Spawnable = true )]
public partial class SkartEntity : CarEntity
{
	public override void Spawn()
	{
		base.Spawn();

		var modelName = "models/apc_fixed.vmdl";

		SetModel( modelName );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
		SetInteractsExclude( CollisionLayer.Player );
		EnableSelfCollisions = false;

		var trigger = new ModelEntity
		{
			Parent = this,
			Position = Position,
			Rotation = Rotation,
			EnableTouch = true,
			CollisionGroup = CollisionGroup.Trigger,
			Transmit = TransmitType.Never,
			EnableSelfCollisions = false,
		};

		trigger.SetModel( modelName );
		trigger.SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
	}
}
