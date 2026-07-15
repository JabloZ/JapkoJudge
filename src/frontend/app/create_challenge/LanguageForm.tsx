'use client';
export default function LanguageForm(){
    return(
    
    <form>
        <input type="text" placeholder="Startcode"/>
        <label htmlFor="startfile">Select file:</label>
        <input type="file" name="startfile" id="startfile"/>
        
        <input type="text" placeholder="Testfile"/>
        <label htmlFor="testfile">Select file:</label>
        <input type="file" name="testfile" id="testfile"/>
    </form>);
}