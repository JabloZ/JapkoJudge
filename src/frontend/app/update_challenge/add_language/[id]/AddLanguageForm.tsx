'use client';

import { useState } from "react";
import { useActionState } from "react";
import { AddLanguageHandle } from "./actions";
export function AddLanguageForm({id}:{id: string}){
    const actionId=AddLanguageHandle.bind(null,id);
    const [state,formAction,isPending]=useActionState(actionId,null);
    const [selectedLanguage, setSelectedLanguage]=useState('');
    return(
        <div>
            <form action={formAction}>
                <label htmlFor="languages">Choose language</label>
                <select name="language" id="languages" value={selectedLanguage} onChange={(e)=>setSelectedLanguage(e.target.value)}>
                    <option value="c">C (.c)</option>
                    <option value="py">Python (.py)</option>
                </select>
                <label htmlFor="startfile">Select file:</label>
                <input type="file" name="startfile" id="startfile"/>
                
                <label htmlFor="testfile">Select file:</label>
                <input type="file" name="testfile" id="testfile"/>
                <button type="submit">Submit</button>
            </form>
            
        </div>
    );
}