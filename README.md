# OET
2D square mesh generator for OLM applications.


<p align="center">
  <img src="https://raw.githubusercontent.com/elpekozgun/RayTracer/master/RayTracer/samples/1.PNG" alt="drawing" width="512"/>
</p>


 In order to get accurate results from simulation and modeling problems, the inputs of the problems should be well defined and prepared. However, generating inputs on some problem types can be a cumbersome task to do efficiently in terms of time and performance. There are even times where preparing a proper configuration take more time than solving the problem itself. To eliminate this roadblock, I made the program called OET to generate inputs for OLM solver program which was programmed by an Instructor on my BS department that I respect a lot. OET is a program that uses a graphical interface where user can draw CAD based objects in 2D environment and prepare a meshed configuration with loading and supports included. OET mesher includes GNUPlot integration, which is the number one choice in scientific program visualization.
 
 ## Mesher Tool
 
  OET mesher has 3 main different CAD objects, Area, line and dot. These 3 elements are related to Concrete Area, steel reinforcement and special joints respectively. OET mesher has input settings as mesh-size, the Horizon for element connectivity generation and boundary conditions for ground. When mesher engine runs, it generates point using cad objects present in the system. Points have material properties depending on what kind of element it was generated by. After generating the point data, OET prepares a lattice model using various geometry generation and manipulation algorithms. Point elements are connected to each other depending on their material properties, orientation, distance compared to horizon and finally by adhering to which specific objects they were created by. Finally, this prepared model can be viewed by using GnuPlot, and is used directly by OLM solver application.

## Methodology

 OET mesher has 3 main different CAD objects, Area, line and dot. These 3 elements are related to Concrete Area, steel reinforcement and special joints respectively. OET mesher has input settings as mesh-size, the Horizon for element connectivity generation and boundary conditions for ground. When mesher engine runs, it generates point using cad objects present in the system. Points have material properties depending on what kind of element it was generated by. After generating the point data, OET prepares a lattice model using various geometry generation and manipulation algorithms. Point elements are connected to each other depending on their material properties, orientation, distance compared to horizon and finally by adhering to which specific objects they were created by. Finally, this prepared model can be viewed by using GnuPlot, and is used directly by OLM solver application.
 
 ## Mesh Elements
**-Nodes:**
     Nodes are the main elements used in 2D frame object creation. They have 2 different material type, concrete and steel. They contain information on their thickness, rebar size, and rebar count. Depending on these properties, nodes are used to create different types of frame elements. Nodes also contain the load information.
**-Frames:**
     There are 2 different type of frames namely concrete and steel. They are obtained comparing the materials of the 2 node that are created from. If material of both nodes is steel, then the generated frame element will be a reinforcement bar, if any of the materials is concrete, then the generated frame will be a concrete element. If both nodes have different type of material, the modulus of elasticity of concrete frame is interpolated linearly.
Nodes have reinforcement bar count, reinforcement bar size, and thickness properties on them as well. Nodes are assumed to be reside in concrete always, thus they always have a thickness property present. However, there are cases where in 2D a Node can lie in a location such that, for example a T shaped beam, it may have a Y value right at the connection of Stub and head of T shape. In this case the thickness of the node is interpolated using these 2 different thickness values. Reinforcement bar count, and size is only in importance if nodes that create the frame are both steel. Below figures shows the concept of frame generations explained in this section.

## Cad Objects

 **-Grid**:
    Grid is the base 2D background element present for making it easy for users to properly create related geometries. Grid have 3 different properties, height, length and size of elements. Since height and length of objects is equal in orthogonal manner in OLM applications, there is only a single grid-size property available. It is strongly suggested for users to adapt their grid that can allow them to snap the points on the actual 2D location of the object they want to create.
    
**-item Concrete:**
    When concrete tool is activated, user can click on various locations on the canvas to create a polygonal region. This region represents a concrete area within the structure. When mesher runs, it creates concrete nodes reside within these concrete areas. For example, for a concrete column with B = 20cm, and L = 100cm, and with a meshsize of 5cm, mesher creates 105 concrete nodes for this concrete area representing the concrete column. It is encouraged for users to activate snap option when using this tool, in order to eliminate round-off errors in creating a geometry.
    
**-Bar:**
    reinforcement bar tool is a tool used to create line objects that represent rebars. A single rebar object has diameter and rebar count properties. Diameter is the regular diameter of the rebar object. Count represents the amount of rebars through the Z axis of the structure. Since this is a 2D application, rebars are rather treated as batches with a count in Z, rather than single elements. The main assumption is that the rebars should be inside concrete, thus mesher can create rebar nodes only if rebar elements are surrounded by some concrete region.
    
 **-Point:**
    Special point tool simply creates point objects. Node objects are used in two different ways to  
    -Apply point loads in X, Y direction.
    -Assign support or constraints.
    
**-GnuPlot** is a copyrighted but freely distributed cross platform go-to application for visualizing simulation and scientific data. OET application outputs are arranged and generated in a way that they can be run on GNUPlot without problems. GnuPlot in this research is used to visualize the outputs of the OET mesher, and also for the test results. GNUPlot is integrated inside OET mesher so that the outputs can be run directly using the OET mesher tool.
    
