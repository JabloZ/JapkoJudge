'use client';
import LanguageForm from "./LanguageForm";
import { useState } from "react";
export function ChallengeForm(){
    
    return(
        <div>
            
            <form>
                <input type="text" name="title" placeholder="Title"/>
                <input type="text" name="title" placeholder="Description"/>
            </form>
            <p>You will be able to add another languages later in Edit panel.<br></br>
                For now add first language supported</p>
            <br></br>
            <LanguageForm/>
            
        </div>
    );
}