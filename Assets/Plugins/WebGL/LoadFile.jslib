var loadFile = {
    LoadFileFromBrowser: function(objectNamePtr, funcNamePtr) {
      window.fileLoader = window.fileLoader || {
         busy: false,
         initialized: false,
         rootDisplayStyle: null,  // style to make root element visible
         root_: null,             // root element of form
         ctx_: null,              // canvas for getting image data;
      };
      var g = window.fileLoader;
      if (g.busy) {
          // Don't let multiple requests come in
          return;
      }
      g.busy = true;

      var objectName = UTF8ToString(objectNamePtr);
      var funcName = UTF8ToString(funcNamePtr);

      if (!g.initialized) {
          g.initialized = true;
          g.ctx = window.document.createElement("canvas").getContext("2d");

          // Append a form to the page (more self contained than editing the HTML?)
          g.root = window.document.createElement("div");
          g.root.innerHTML = [
            '<style>                                                    ',
            '.loadFile {                                                ',
            '    position: absolute;                                    ',
            '    left: 0;                                               ',
            '    top: 0;                                                ',
            '    width: 100%;                                           ',
            '    height: 100%;                                          ',
            '    display: -webkit-flex;                                 ',
            '    display: flex;                                         ',
            '    -webkit-flex-flow: column;                             ',
            '    flex-flow: column;                                     ',
            '    -webkit-justify-content: center;                       ',
            '    -webkit-align-content: center;                         ',
            '    -webkit-align-items: center;                           ',
            '                                                           ',
            '    justify-content: center;                               ',
            '    align-content: center;                                 ',
            '    align-items: center;                                   ',
            '                                                           ',
            '    z-index: 2;                                            ',
            '    color: white;                                          ',
            '    background-color: rgba(0,0,0,0.75);                    ',
            '    font: sans-serif;                                      ',
            '    font-size: x-large;                                    ',
            '}                                                          ',
            '.loadFile a,                                               ',
            '.loadFile label {                                          ',
            '   font-size: x-large;                                     ',
            '   background-color: 3C0073;                               ',
            '   border-radius: 0.5em;                                   ',
            '   border: 1px solid black;                                ',
            '   padding: 0.5em;                                         ',
            '   margin: 0.25em;                                         ',
            '   outline: none;                                          ',
            '   display: inline-block;                                  ',
            '}                                                          ',
            '.loadFile input {                                          ',
            '    display: none;                                         ',
            '}                                                          ',
            '</style>                                                   ',
            '<div class="loadFile">                                     ',
            '    <div>                                                  ',
            '      <label for="f">Click to choose a file</label>  ',
            '      <input id="f" type="file" accept=".ldc"/><br/>',
            '      <a>Cancel</a>                                        ',
            '    </div>                                                 ',
            '</div>                                                     ',
          ].join('\n');
          var input = g.root.querySelector("input");
          input.addEventListener('change', getPic);

          // prevent clicking in input or label from canceling
          input.addEventListener('click', preventOtherClicks);
          var label = g.root.querySelector("label");
          label.addEventListener('click', preventOtherClicks);

          // clicking cancel or outside cancels
          var cancel = g.root.querySelector("a");  // there's only one
          cancel.addEventListener('click', handleCancel);
          var loadFile = g.root.querySelector(".loadFile");
          loadFile.addEventListener('click', handleCancel);

          // remember the original style
          g.rootDisplayStyle = g.root.style.display;

          window.document.body.appendChild(g.root);
      }

      // make it visible
      g.root.style.display = g.rootDisplayStyle;

      function preventOtherClicks(evt) {
          evt.stopPropagation();
      }

      function getPic(evt) {
          evt.stopPropagation();
          var fileInput = evt.target.files;
          if (!fileInput || !fileInput.length) {
              return sendError("no file selected");
          }

          var reader = new FileReader();
          var thisInput = this;

          reader.onload = function(evt)
          {
              if (evt.target.readyState != 2)
                  return;

              if (evt.target.error)
              {
                  sendError("Error while reading file " + loadEvent.target.error);
                  return;
              }

              sendResult(evt.target.result);
          }
          reader.readAsText(fileInput[0]);
      }

      function handleCancel(evt) {
          evt.stopPropagation();
          evt.preventDefault();
          sendError("cancelled");
      }

      function sendError(msg) {
          sendResult("error: " + msg);
      }

      function hide() {
          g.root.style.display = "none";
      }

      function sendResult(result) {
          hide();
          g.busy = false;
          SendMessage(objectName, funcName, result);
          document.getElementById('file').value = '';
      }
    },
};

mergeInto(LibraryManager.library, loadFile);


