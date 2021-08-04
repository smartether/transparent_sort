using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Transparent_Sort_Feature : MonoBehaviour {
	public enum PassType
    {
		MaskRoleTransparentArea = 0,
		ResetMaskedDepth = 1,
		DrawOverlayInfo = 2,
		DrawTransparent = 3,
		RestoreDepth = 4
    }
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, SetupCommandBuff());
	}

	Camera cam;
	public Mesh RoleModel;
	public Mesh PlaneMesh;
	public Material PrestencilMat;
	public int[] layerRef;

	public Vector3 modelRotation = new Vector3(-90, 90, 0);




	CommandBuffer SetupCommandBuff()
	{
		Matrix4x4 roleModelMatrix = Matrix4x4.TRS(new Vector3(0, -2, 3), Quaternion.Euler(modelRotation), Vector3.one * 500);
		Matrix4x4 planeMeshMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0), Vector3.one * 100);

		CommandBuffer cb = new CommandBuffer();


		Material[] mpb = new Material[layerRef.Length];
		// draw opaque pass
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 4);

		for (int i = 0,c= layerRef.Length; i < c; i++)
		{
			// reset depth
			cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
			// mask rolemodel stencil
			cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 0);
			// draw occlusion info
			cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 2);
			// reset depth
			cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
			// draw opaque pass restore depth
			cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 5);

			mpb[i] = new Material(PrestencilMat);
			mpb[i].SetFloat("_DrawLayerRef", (float)layerRef[i]);
			mpb[i].SetInt("_DrawLayerRef", layerRef[i]);
			cb.DrawMesh(RoleModel, roleModelMatrix, mpb[i], 0, 3);

		}

		//// reset depth
		//cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
		//// draw mid layer
		//cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 3);
		//// reset depth
		//cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
		//// draw nearest layer
		//cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 3);

		return cb;
	}
	
}
