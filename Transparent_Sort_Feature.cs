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
	Matrix4x4 roleModelMatrix = Matrix4x4.TRS(new Vector3(0, -2, 3), Quaternion.Euler(-90, 90, 0), Vector3.one * 500);
	Matrix4x4 planeMeshMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0), Vector3.one * 100);


	delegate void IssueEvent(int evt);

	void doIssueEvent(int evt)
    {
		roleModelMatrix = roleModelMatrix * Matrix4x4.Rotate(Quaternion.Euler(0, 5, 0));
    }

	CommandBuffer SetupCommandBuff()
    {
		CommandBuffer cb = new CommandBuffer();


		Material[] mpb = new Material[layerRef.Length];
		// mask rolemodel stencil
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 0);
		// draw opaque pass
		cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 4);

		cb.IssuePluginEvent(System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate((IssueEvent)doIssueEvent), 0);

		for (int i = 0,c= layerRef.Length; i < c; i++)
		{
			// reset depth
			cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
			// draw occlusion info
			cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 2);
			// reset depth
			cb.DrawMesh(PlaneMesh, planeMeshMatrix, PrestencilMat, 0, 1);
			// draw opaque pass restore depth
			cb.DrawMesh(RoleModel, roleModelMatrix, PrestencilMat, 0, 5);

			mpb[i] = new Material(PrestencilMat);
			mpb[i].SetFloat("_DrawLayerRef", (float)layerRef[i]);
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
