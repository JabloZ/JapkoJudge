'use client';

import { useState } from "react";
import { useActionState } from "react";
import { EditChallengeGeneralRequest, GetChallengeGeneralData } from "./actions";
export function EditChallengeForm({id, title, description}:{id:string, title:string,description:string}){
    const editChallengeGeneralRequestId=EditChallengeGeneralRequest.bind(null,id);
    const [state,formAction,isPending]=useActionState(editChallengeGeneralRequestId,null);
    const [titleValue, setTitleValue]=useState(title);
    const [descriptionValue, setDescriptionValue]=useState(description);
    return(
        <div>
            <form action={formAction}>
                <input type="text" name="title" placeholder="Title" value={titleValue} onChange={(e)=>setTitleValue(e.target.value)}/>
                <input type="text" name="description" placeholder="Description" value={descriptionValue} onChange={(e)=>setDescriptionValue(e.target.value)}/>
                <br></br>
                <button type="submit">Submit</button>
            </form>
        </div>
    );
}