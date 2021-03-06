using Sandbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerAnimator : PawnAnimator
{
	public Rotation targetRot;

	private Rotation lerpRotation;

	public override void Simulate()
	{
		var idealRotation = Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );

		DoRotation( idealRotation );
		DoWalk();

		//
		// Let the animation graph know some shit
		//

		SetParam( "b_grounded", GroundEntity != null );

		//local vec = (position - bot:GetPos() ):GetNormal():Angle().y
		//local myAngle = bot:EyeAngles().y

		float modelRotation = Rotation.Forward.Normal.EulerAngles.yaw;
		float lookRotation = Input.Rotation.Forward.Normal.EulerAngles.yaw;

		float rotationDifference = modelRotation - lookRotation;

		if ( rotationDifference > 180 )
			rotationDifference -= 360;
		if ( rotationDifference < -180 )
			rotationDifference += 360;

		//Log.Info( modelRotation + " : " + lookRotation + " :: " + rotationDifference );
		//Log.Info( Input.Rotation.Pitch() );

		SetParam( "aim_pitch", Input.Rotation.Pitch() );
		SetParam( "aim_yaw", rotationDifference );

		var player = Local.Pawn as Player;
		if ( player == null )
			return;

		WalkController controller = player.Controller as WalkController;

		//Log.Info( controller.Duck.IsActive );
		SetParam( "b_crouch", controller.Duck.IsActive );
	}

	public virtual void DoRotation( Rotation idealRotation )
	{
		float turnSpeed = 10.0f;

		//
		// Our ideal player model rotation is the way we're facing
		//
		var allowYawDiff = 60;

		//
		// If we're moving, rotate to our ideal rotation
		//
		if ( Velocity.Length <= 0 && GroundEntity != null )
		{
			if ( Vector3.GetAngle( Rotation.Forward, idealRotation.Forward ) > allowYawDiff )
				lerpRotation = idealRotation;
		}
		else
			lerpRotation = idealRotation;

		Rotation = Rotation.Lerp( Rotation, lerpRotation, turnSpeed * Time.Delta );
	}

	void DoWalk()
	{
		// Move Speed
		{
			var dir = Velocity;
			var forward = Rotation.Forward.Dot( dir );
			var sideward = Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			SetParam( "move_direction", angle );
			SetParam( "move_speed", Velocity.Length );
			SetParam( "move_groundspeed", Velocity.WithZ( 0 ).Length );
			SetParam( "move_y", sideward );
			SetParam( "move_x", forward );
			SetParam( "move_z", Velocity.z );

			//Log.Info( Velocity.Length );
		}
	}
}
