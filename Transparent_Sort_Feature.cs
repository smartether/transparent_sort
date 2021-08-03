using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Transparent_Sort_Feature : MonoBehaviour {

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, SetupCommandBuff());
	}

	Camera cam;
	public Mesh RoleModel;
	public Mesh PlaneMesh;
	public Material PrestencilMat;

	CommandBuffer SetupCommandBuff()
    {
		Matrix4x4 roleModelMatrix = Matrix4x4.TRS(new Vector3(0, -2, 3), Quaternion.Euler(-90, 90, 0), Vector3.one * 500);
		Matrix4x4 planeMeshMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0), Vector3.one * 100);
		CommandBuffer cb = new CommandBuffer();
		// mask rolemodel stencil
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 0);
		// reset depth
		cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);

		// draw occlusion info
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 2);

		// draw faraest layer
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 3);
		// reset depth
		cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
		// draw mid layer
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 3);
		// reset depth
		cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
		// draw nearest layer
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 3);

		return cb;
	}
	
}
