import React from 'react';

const Comment = (props) => {
    return (
        <li>
            <div className="comment">
                <div className="user-avatar">

                </div>
                <div className="text-comment-container">
                    <p className="elem-text-medium">{props.username}</p>
                    <p className="comment-text elem-text-extrasmall">{props.commentText}</p>
                </div>
            </div>
        </li>
    );
};

export default Comment;