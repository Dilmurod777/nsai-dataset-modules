using UnityEngine;

namespace Instances
{
	public class ObjectMeta : MonoBehaviour
	{
		public enum AttachTypes
		{
			SmoothInstall,
			StepScrew,
			SmoothScrew,
			StepInstall
		}

		public enum RotationAxisEnum
		{
			X,
			Y,
			Z,
			NegX,
			NegY,
			NegZ
		}

		public enum Status
		{
			Initial,
			Attached,
			Dettached
		}

		public RotationAxisEnum attachRotationAxis;
		public RotationAxisEnum dettachRotationAxis;
		public AttachTypes attachType;
		public AttachTypes detachType;
		public Status status = Status.Initial;
	}
}