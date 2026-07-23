'use client';

import { useState } from "react";
import { useActionState } from "react";
import { EditLanguageHandle } from "./actions";
export function EditLanguageForm({id, language_id}:{id: string, language_id:string}){
    const actionId=EditLanguageHandle.bind(null,id,language_id);//useactionstate calls with 2 args, so we add id before call
    const [state,formAction,isPending]=useActionState(actionId,null);
    //todo - print language name not id
    return(
        <div>
            <form action={formAction}>
                <p>editing for language {language_id}</p>
                <label htmlFor="startfile">Select file:</label>
                <input type="file" name="startfile" id="startfile"/>
                
                <label htmlFor="testfile">Select file:</label>
                <input type="file" name="testfile" id="testfile"/>
                <button type="submit">Submit</button>
            </form>
            
        </div>
    );
}

