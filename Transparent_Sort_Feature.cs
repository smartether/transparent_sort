using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Transparent_Sort_Feature : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	public Mesh RoleModel;
	public Material PrestencilMat;

	CommandBuffer SetupCommandBuff()
    {
		CommandBuffer cb = new CommandBuffer();
		// mask rolemodel stencil
		cb.DrawMesh(RoleModel, Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 0, 0);
		// reset depth
		cb.DrawProcedural(Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 1, MeshTopology.Quads, 4);
		// draw faraest layer
		cb.DrawMesh(RoleModel, Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 0, 0);
		// reset depth
		cb.DrawProcedural(Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 1, MeshTopology.Quads, 4);
		// draw mid layer
		cb.DrawMesh(RoleModel, Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 0, 0);
		// reset depth
		cb.DrawProcedural(Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 1, MeshTopology.Quads, 4);
		// draw nearest layer
		cb.DrawMesh(RoleModel, Matrix4x4.TRS(Vector3.forward * 5, Quaternion.identity, Vector3.one * 20), PrestencilMat, 0, 0);

	}
	
}
