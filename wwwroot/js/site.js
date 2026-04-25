// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function build(element = document.createElement("div"), htmlTree, infoMap = new Map()) {
    let mainContainer = document.createElement("div");
    mainContainer.classList.add("w-100", "d-flex");
    mainContainer.style = "min-width: fit-content;";
    element.appendChild(mainContainer);
    let currentNode = htmlTree.Nodes[htmlTree.RootId];
    let nodeStack = [currentNode];
    let childContainerMap = new Map();
    while (nodeStack.length > 0) {
        // Get node from stack
        currentNode = nodeStack.pop()

        // Make new element
        let newElement = document.createElement("div");
        newElement.classList.add("flex-grow-1");

        // Append to parent
        if (currentNode.Parent == -1) {
            mainContainer.appendChild(newElement);
        }
        else {
            childContainerMap[currentNode.Parent].appendChild(newElement);
            if (htmlTree.Nodes[currentNode.Parent].Parent == -1) {
                // do nothing
            }
            else if (htmlTree.Nodes[currentNode.Parent].Children.length == 1) {
                let topEdge = document.createElement("div");
                topEdge.classList.add("vertical-edge");
                newElement.appendChild(topEdge);
            }
            else {
                let edgeDiv = document.createElement("div");
                edgeDiv.classList.add("d-grid");
                newElement.appendChild(edgeDiv);
                let i = htmlTree.Nodes[currentNode.Parent].Children.findIndex((j) => j == currentNode.Index);
                if (i > 0 && i < htmlTree.Nodes[currentNode.Parent].Children.length - 1) {
                    let topLeftEdge = document.createElement("div");
                    let topRightEdge = document.createElement("div");
                    topLeftEdge.classList.add("top-to-left-edge");
                    topRightEdge.classList.add("top-to-right-edge");
                    topLeftEdge.style = "grid-column: 1;"
                    topRightEdge.style = "grid-column: 2;"

                    edgeDiv.appendChild(topLeftEdge);
                    edgeDiv.appendChild(topRightEdge);
                }
                else if (i > 0) {
                    let topLeftEdge = document.createElement("div");
                    let topRightEdge = document.createElement("div");
                    topRightEdge.classList.add("only-top-to-right-edge");
                    topLeftEdge.style = "grid-column: 1;"
                    topRightEdge.style = "grid-column: 2;"

                    edgeDiv.appendChild(topLeftEdge);
                    edgeDiv.appendChild(topRightEdge);
                }
                else {
                    let topLeftEdge = document.createElement("div");
                    let topRightEdge = document.createElement("div");
                    topLeftEdge.classList.add("only-top-to-left-edge");
                    topLeftEdge.style = "grid-column: 1;"
                    topRightEdge.style = "grid-column: 2;"
                    edgeDiv.appendChild(topLeftEdge);
                    edgeDiv.appendChild(topRightEdge);
                }
            }

            // Create info container
            let infoContainer = document.createElement("div");
            infoContainer.classList.add("w-100", "px-2");
            newElement.appendChild(infoContainer);

            let infos = document.createElement("div");
            infos.classList.add("node");
            infoContainer.appendChild(infos);

            infoMap[currentNode.Index] = infos;

            // Create infos
            let tagText = document.createElement("p");
            tagText.textContent = "<" + currentNode.Tag + ">";
            tagText.classList.add("m-auto")
            infos.appendChild(tagText);
            let separator = document.createElement("hr");
            separator.classList.add("my-1")
            infos.appendChild(separator);

            if (currentNode.Id != "") {
                let idText = document.createElement("p");
                idText.textContent = "id: " + currentNode.Id;
                idText.classList.add("text-start", "my-0");
                infos.appendChild(idText);
            }

            if (currentNode.Class.length > 0) {
                let classText = document.createElement("p");
                classText.textContent = "class: " + currentNode.Class;
                classText.classList.add("text-start", "my-0");
                infos.appendChild(classText);
            }

            if (currentNode.Attribute.length > 0) {
                let attributeText = document.createElement("p");
                attributeText.textContent = "attributes: " + currentNode.Attribute;
                attributeText.classList.add("text-start", "my-0");
                infos.appendChild(attributeText);
            }
            
            if (currentNode.Children.length > 0) {
                let bottomEdge = document.createElement("div");
                bottomEdge.classList.add("vertical-edge");
                newElement.appendChild(bottomEdge);
            }
        }

        // Create child container
        let childContainer = document.createElement("div");
        newElement.appendChild(childContainer);
        childContainer.classList.add("w-100", "d-flex");
        childContainer.style = "min-width: fit-content;";
        childContainerMap[currentNode.Index] = childContainer;

        // Next Elements
        for (let i = currentNode.Children.length - 1; i >= 0; i--) {
            let index = currentNode.Children[i];
            let nextNode = htmlTree.Nodes[index];
            nodeStack.push(nextNode);
        }
    }
}

function SanitizeJsonString(rawString) {
    return rawString
    .replace(/[\x00-\x1F\x7F-\x9F]/g, (chr) => {
      // Map common control characters to their escaped versions
      const charMap = {
        '\b': '\\b',
        '\f': '\\f',
        '\n': '\\n',
        '\r': '\\r',
        '\t': '\\t'
      };
      return charMap[chr] || ''; // Replace others with an empty string
    });
};
