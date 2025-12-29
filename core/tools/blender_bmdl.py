import bpy
import struct
import math
import os
path = r"E:\boxdraw\game\models\characters\human_base.bmdl"
scale = 100

with open(path, 'wb') as f:
    meshes = []
    for mesh in bpy.context.selectable_objects:
        if mesh.type != 'MESH':
            continue
        print(mesh)
        meshes.append(mesh)
    armature = None
    for mesh in meshes:
        for modifier in mesh.modifiers:
            if modifier.type != 'ARMATURE':
                continue
            armature = modifier.object
        if armature:
            break
    if armature:
        f.write(struct.pack('<H', len(bpy.data.armatures[armature.name].bones))) #num bones
        for bone in bpy.data.armatures[armature.name].bones:
            f.write(struct.pack('<B', len(bone.name))) #bone name length
            f.write(bone.name.encode('ascii')) #bone name
            m = bone.matrix_local
            if bone.parent:
                m = bone.parent.matrix_local.inverted() @ m
            #6 float bone transform relative to parent
            pos = m.to_translation() * scale
            f.write(struct.pack('<f', pos.x))
            f.write(struct.pack('<f', pos.y))
            f.write(struct.pack('<f', pos.z))
            rot = m.to_euler()
            f.write(struct.pack('<f', math.degrees(rot.x)))
            f.write(struct.pack('<f', math.degrees(rot.y)))
            f.write(struct.pack('<f', math.degrees(rot.z)))
    else:
        f.write(struct.pack('<H', 0)) #num bones
    f.write(struct.pack('<B', len(meshes))) #num meshes
    for mesh in meshes:
        vertexdict = {}
        verticies = []
        indicies = []
        m = mesh.to_mesh(preserve_all_data_layers=True)
        m.calc_loop_triangles()
        for tri in m.loop_triangles:
            for i in range(3):
                v = mesh.data.vertices[tri.vertices[i]]
                uv = mesh.data.uv_layers.active.data[tri.loops[i]].uv
                hc = hash((tri.vertices[i], uv.x, uv.y))
                if hc in vertexdict:
                    indicies.append(vertexdict[hc])
                    continue
                vertex = {
                    "pos" : v.co * scale,
                    "normal" : v.normal,
                    "uv" : uv,
                    "groups" : v.groups,
                }
                vertexdict[hc] = len(verticies)
                verticies.append(vertex)
                indicies.append(vertexdict[hc])
        mesh.to_mesh_clear()
        f.write(struct.pack('<B', len(mesh.name))) #mesh name length
        f.write(mesh.name.encode('ascii')) #mesh name
        f.write(struct.pack('<B', len(mesh.material_slots[0].name))) #material name length
        f.write(mesh.material_slots[0].name.encode('ascii')) #material name
        f.write(struct.pack('<B', 1)) #uv channel count
        f.write(struct.pack('<B', 0)) #vertex color channel count
        f.write(struct.pack('<I', len(indicies))) #num indicies
        for index in indicies:
            f.write(struct.pack('<I', index))
        f.write(struct.pack('<I', len(verticies))) #num verticies
        for vertex in verticies:
            f.write(struct.pack('<f', vertex["pos"].x))
            f.write(struct.pack('<f', vertex["pos"].y))
            f.write(struct.pack('<f', vertex["pos"].z))
            f.write(struct.pack('<f', vertex["normal"].x))
            f.write(struct.pack('<f', vertex["normal"].y))
            f.write(struct.pack('<f', vertex["normal"].z))
            f.write(struct.pack('<f',  vertex["uv"].x))
            f.write(struct.pack('<f',1-vertex["uv"].y))
            f.write(struct.pack('<B', min(len(vertex["groups"]), 4))) #vertex group count
            for i in range(min(len(vertex["groups"]), 4)):
                group = vertex["groups"][i]
                boneindex = 0
                for bone in bpy.data.armatures[armature.name].bones:
                    if bone.name == mesh.vertex_groups[group.group].name:
                        break
                    boneindex += 1
                f.write(struct.pack('<H', boneindex)) #bone index
                f.write(struct.pack('<f', group.weight)) #bone index