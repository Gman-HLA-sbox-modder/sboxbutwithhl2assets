using System.Collections.Generic;
using System;
using Sandbox;

  [Library("ent_rollermine", Title = "Rollermine", Spawnable = true )]
  public class MelonEntity : BasePhysics {

    // lifetime

    internal static HashSet<MelonEntity> AllMelons = new HashSet<MelonEntity>();
    //internal static HashSet<Entity> Targets = new HashSet<Entity>();

    public override void Spawn() {
      base.Spawn();

      SetModel("models/roller_vehicledriver.vmdl");

      AllMelons.Add(this);
    }

    protected override void OnDestroy() {
      base.OnDestroy();

      AllMelons.Remove(this);
    }

    // target

    private Entity Target {get; set;}
    private DateTime LastTargetRefresh {get; set;}

    private Entity FindTarget() {
      Entity closest = null;
      foreach (var ent in Entity.All) {
        if (!IsValidTarget(ent)) continue;
        if (closest == null || this.Position.Distance(closest.Position) > this.Position.Distance(ent.Position)) {
          closest = ent;
        }
      }
      return closest;
    }

    protected bool IsValidTarget(Entity ent) {
      if (!ent.IsValid()) return false;
      if (ent is Player && ent.Health <= 0) return false;
      if (ent is Prop) {

      }
		return ent is Player;
    }

    // tick

    [Event.Tick.Server]
    private void Tick() {
      if (LastTargetRefresh == null || (DateTime.Now - LastTargetRefresh).Seconds >= 1) {
        Target = FindTarget();
        LastTargetRefresh = DateTime.Now;
      }
      if (IsValidTarget(Target)) {
        PhysicsBody.ApplyForceAt(Position + new Vector3 {z = 580}, (Target.Position - this.Position).Normal*2000);
      }
    }

    // create melons

    public static Boolean CanCreateMelon() {
      return AllMelons.Count < SandboxGame.MaxMelons;
    }

    public static MelonEntity CreateMelon(Vector3 pos) {
      return new MelonEntity
	  {
          Position = pos
      };
    }

    protected override void OnPhysicsCollision(CollisionEventData eventData) {

      var ent = eventData.Entity;
      if (!IsValidTarget(ent)) return;

      if (ent is Player) {
        var pos = ent.Position;
        ent.TakeDamage(new DamageInfo {
          Damage = ent.Health,
          Attacker = this
        });
        if (CanCreateMelon()) {
        }
      } else if (CanCreateMelon()) {
        var pos = ent.Position;
        ent.Delete();
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage = 10;
		}

		base.TakeDamage( info );
	}

	public override void OnKilled() 
	{ 
	   base.OnKilled();
		
	   PlaySound( "kersplat" );
	}

	// Make this killable
}
