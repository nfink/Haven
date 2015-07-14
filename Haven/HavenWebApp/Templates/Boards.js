{{~it :board:index}}
<div class="board tile fg-white bg-black"
data-role="tile"
boardid="{{=board.Id}}"
boardicon="{{=board.Image.Filepath}}"
boardname="{{=board.Name}}"
boarddescription="{{=board.Description}}">
<div class="tile-content slide-down">
    <div class="slide">
        <div style="width: 100%;" class="image-container image-format-square">
            <div class="frame">
                <div style="width: 100%; height: 150px; background-image: url('{{=board.Image.Filepath}}'); background-size: cover; background-repeat: no-repeat; border-radius: 0px;">
                </div>
            </div>
        </div>
    </div>
    <div class="slide-over op-cyan text-small padding10">{{=board.Description}}</div>
    <div class="tile-label">{{=board.Name}}</div>
</div>
</div>
{{~}}