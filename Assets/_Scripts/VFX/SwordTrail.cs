using UnityEngine;

[ExecuteAlways]
public class SwordTrail : MonoBehaviour 
{
	[SerializeField] private Transform[] m_Top;
	[SerializeField] private Transform[] m_Bottom;

	[SerializeField] private int frames = 9;
	[SerializeField, Range(0, 1f)] private float m_DebugLerp;

	private Vector3[] vertices;
	private Vector2[] uvs;
	private int[] triangles;

	private Mesh mesh;



	[ContextMenu("Create Mesh")]
	private void Start()
	{
		mesh = new Mesh();
		mesh.name = "Trail";
		GetComponent<MeshFilter>().mesh = mesh;

		vertices = new Vector3[2 + frames * 2];
		triangles = new int[frames * 2 * 3];
		uvs = new Vector2[vertices.Length];

		var topVertices = new Vector3[m_Top.Length];

		for (int i = 0; i < topVertices.Length; i++)
		{
			topVertices[i] = m_Top[i].position;
		}

		var botVertices = new Vector3[m_Bottom.Length];

		for (int i = 0; i < botVertices.Length; i++)
		{
			botVertices[i] = m_Bottom[i].position;
		}

		for (int i = 0; i < vertices.Length; i += 2)
        {
			vertices[i] = transform.InverseTransformPoint(Bezier(botVertices, (i / 2f) / frames));
			vertices[i + 1] = transform.InverseTransformPoint(Bezier(topVertices, (i / 2f) / frames));
		}

		for (int i = 0; i < vertices.Length; i += 2)
		{
			uvs[i] = new Vector2(0f, (float)i / vertices.Length);
			uvs[i + 1] = new Vector2(1f, (float)i / vertices.Length);

			if (i > 1)
			{
				int t = (i - 2) * 3;

				triangles[t] = i - 2;
				triangles[t + 1] = i - 1;
				triangles[t + 2] = i;
				t += 3;
				triangles[t] = i + 1;
				triangles[t + 1] = i + 1 - 1;
				triangles[t + 2] = i + 1 - 2;
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
	}

    void LateUpdate()
	{
		for (int i = 0; i < vertices.Length; i += 2)
		{
			vertices[i] = transform.InverseTransformPoint(Bezier(m_Bottom, (i / 2f) / frames));
			vertices[i + 1] = transform.InverseTransformPoint(Bezier(m_Top, (i / 2f) / frames));
		}

		mesh.vertices = vertices;
    }

    private void OnDrawGizmos()
    {
		DrawBezier(m_Top);
		DrawBezier(m_Bottom);
	}

	private void DrawBezier(Transform[] anchors)
    {
		Gizmos.color = Color.green;

		var positions = new Vector3[anchors.Length];

		for (int i = 0; i < positions.Length; i++)
		{
			positions[i] = anchors[i].position;
		}

		var pos = Bezier(positions, m_DebugLerp);

		for (int i = 0; i < positions.Length; i++)
		{
			Gizmos.DrawWireSphere(positions[i], 0.1f);

			if (i < positions.Length - 1)
			{
				Gizmos.DrawLine(positions[i], positions[i + 1]);
			}
		}

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(pos, 0.1f);

		for (int i = 0; i < frames; i++)
		{
			var pos1 = Bezier(positions, i / (float)frames);
			var pos2 = Bezier(positions, (i + 1) / (float)frames);

			Gizmos.DrawLine(pos1, pos2);
		}
	}

	private Vector3 Bezier(Vector3[] positions, float lerp)
    {
		Vector3 p01 = Vector3.Lerp(positions[0], positions[1], lerp);
		Vector3 p12 = Vector3.Lerp(positions[1], positions[2], lerp);
		Vector3 p23 = Vector3.Lerp(positions[2], positions[3], lerp);

		Vector3 l0112 = Vector3.Lerp(p01, p12, lerp);
		Vector3 l1223 = Vector3.Lerp(p12, p23, lerp);

		Vector3 final = Vector3.Lerp(l0112, l1223, lerp);

		return final;
	}

	private Vector3 Bezier(Transform[] anchors, float lerp)
	{
		var positions = new Vector3[anchors.Length];

        for (int i = 0; i < positions.Length; i++)
        {
			positions[i] = anchors[i].position;
        }

		Vector3 p01 = Vector3.Lerp(positions[0], positions[1], lerp);
		Vector3 p12 = Vector3.Lerp(positions[1], positions[2], lerp);
		Vector3 p23 = Vector3.Lerp(positions[2], positions[3], lerp);

		Vector3 l0112 = Vector3.Lerp(p01, p12, lerp);
		Vector3 l1223 = Vector3.Lerp(p12, p23, lerp);

		Vector3 final = Vector3.Lerp(l0112, l1223, lerp);

		return final;
	}
}
